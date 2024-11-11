using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
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

        public PdfJsHost(string filePath)
        {
            _sourceFilePath = filePath;
            _fileName = Path.GetFileName(_sourceFilePath);
            _pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfJs", "web", _fileName);
            _allowedUrl = $"https://pdfjs/web/viewer.html?file={Uri.EscapeDataString(_fileName)}";

            CopyFile();
            InitializeWebView();
            Application.Current.Exit += (s, e) => Release();
        }

        void CopyFile()
        {           
            try { File.Copy(_sourceFilePath, _pdfPath, true); }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
        }

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
            this.ExecuteScriptAsync(Scripts.EditButtonsScript());
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
            this.CoreWebView2.SetVirtualHostNameToFolderMapping("pdfjs", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdfjs"), CoreWebView2HostResourceAccessKind.DenyCors);
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
                    var method = this.GetType().GetMethod(actionName, BindingFlags.NonPublic | BindingFlags.Instance);
                    method?.Invoke(this, null); // Calls the method if it exists, passing no parameters
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
        private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
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
                        //if (File.Exists(downloadPath))
                        //{
                        //    var result = MessageBox.Show(
                        //    "File already exists. Do you want to overwrite it?", "Confirm Overwrite",
                        //    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        //    if (result == MessageBoxResult.No) { e.Cancel = true; }
                        //}
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

        void SaveAs()
        {
            try
            {
                isSaveAs = true;
                this.ExecuteScriptAsync("PDFViewerApplication.download();");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Release()
        {
            base.Dispose();
            File.Delete(_pdfPath);
        }
    }
}
