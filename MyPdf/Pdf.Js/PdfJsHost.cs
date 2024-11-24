using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using Pdf.Js;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Pdf.Js
{
    public class PdfJsHost : WebView2
    {
        public string _sourceFilePath;
        public string _pdfPath;
        string _fileName;
        string _allowedUrl;
        bool isSaveAs;

        public PdfJsHost(string filePath, int? pageNumber)
        {
            _sourceFilePath = filePath;
            _fileName = Path.GetFileName(_sourceFilePath);
            _pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pdf.Js", "PdfJs", "web", _fileName);

            if (pageNumber != null) _allowedUrl = $"https://pdfjs/web/viewer.html?file={Uri.EscapeDataString(_fileName)}#page={pageNumber}";
            else _allowedUrl = $"https://pdfjs/web/viewer.html?file={Uri.EscapeDataString(_fileName)}";


            CopyFile();
            InitializeWebView();
            Application.Current.Exit += (s, e) => Release();
        }

        async void CopyFile() => await Task.Run(() =>
        {
            try { File.Copy(_sourceFilePath, _pdfPath, true);   }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }); 

        void InitializeWebView()
        {
            this.CoreWebView2InitializationCompleted += PdfJsHost_CoreWebView2InitializationCompleted;
            this.EnsureCoreWebView2Async();
        }

        private void PdfJsHost_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            //ApplySettings();
            LoadPdf();

            this.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            this.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting; // Add a handler to restrict navigation
            this.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            this.CoreWebView2.WebMessageReceived += Viewer_WebMessageReceived;
        }

        private void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            this.ExecuteScriptAsync(Scripts.EditButtons());
        }

        void ApplySettings()
        {
            this.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            this.CoreWebView2.Settings.AreDevToolsEnabled = false;
            this.CoreWebView2.Settings.IsStatusBarEnabled = false;
            this.CoreWebView2.Settings.IsScriptEnabled = true;
            this.CoreWebView2.Settings.AreHostObjectsAllowed = true;
        }

        void LoadPdf()
        {
                string pdfjsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pdf.Js", "pdfjs");
                this.CoreWebView2.SetVirtualHostNameToFolderMapping("pdfjs", pdfjsFolder,
                        CoreWebView2HostResourceAccessKind.DenyCors);
                this.Source = new Uri(_allowedUrl);
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri != _allowedUrl)  e.Cancel = true;     
        }

        private void Viewer_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = JsonSerializer.Deserialize<Dictionary<string, string>>(e.WebMessageAsJson);
                if (message != null && message.TryGetValue("action", out var actionName))
                {
                    // Use reflection to find and invoke the method by name
                    var method = this.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    method?.Invoke(this, null); // Calls the method if it exists, passing no parameters
                }
                else if (message != null && message.TryGetValue("textExtraction", out var extractedText))
                {
                    ShowExtractedTextResults(extractedText);
                }
                else
                { 
                    Console.WriteLine("No action defined!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async void ShowExtractedTextResults(string extractedText)
        {
            if (!string.IsNullOrEmpty(extractedText))
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.txt");
                await File.WriteAllTextAsync(tempFilePath, extractedText, Encoding.UTF8);
                Process.Start(new ProcessStartInfo  {   FileName = tempFilePath,  UseShellExecute = true  });
            } else  {   MessageBox.Show("Page text result is empty."); }
        }


        private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            if (!e.DownloadOperation.Uri.Contains("blob:https://pdfjs") )
            {
                e.Cancel = true;
                return;
            }

            try
            {
                if (isSaveAs)
                {
                        SaveFileDialog saveFileDialog = new SaveFileDialog
                        {
                            Title = "Save PDF As",
                            Filter = "PDF Files (*.pdf)|*.pdf",
                            FileName = _fileName // Suggest the current file name
                        };

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string downloadPath = saveFileDialog.FileName;
                            if (!downloadPath.EndsWith(".pdf")) downloadPath += ".pdf";
                            _sourceFilePath = downloadPath;
                            e.ResultFilePath = downloadPath; // Set the download location
                        }
                        else
                        {
                            e.Cancel = true;// Cancel download if user cancels the Save As dialog
                        }
                        isSaveAs = false;                        
                }
                else
                {
                    string customDownloadFolder = Path.GetDirectoryName(_sourceFilePath);
                    if (!Directory.Exists(customDownloadFolder)) Directory.CreateDirectory(customDownloadFolder);
                    e.ResultFilePath = _sourceFilePath;
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveAs()
        {
            isSaveAs = true;
            SaveFile();
        }

        public void SaveFile() => this.ExecuteScriptAsync("PDFViewerApplication.download();");

        public async Task<int> GetCurrentPageNumber()
        {
            try
            {
                var pageNumberQuery = await this.ExecuteScriptAsync(Scripts.GetCurrentPageNumber());
                if (int.TryParse(pageNumberQuery, out int currentPageNum))
                {
                    return currentPageNum;
                }
            } catch{ }
            return 0;
        }

        public async void GetTextFromCurrentPageAsync() => await this.ExecuteScriptAsync(Scripts.ExtractTextFromCurrentPage());
        public async void GetTextFromWholeDocAsync() => await this.ExecuteScriptAsync(Scripts.ExtractTextFromWholeDoc());
      

        public void Release()
        {
            base.Dispose();
            File.Delete(_pdfPath);
        }
    }
}



