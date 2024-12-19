using ChromeTabs;
using Microsoft.Web.WebView2.Core;
using Pdf.Js;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Data;
using System.Diagnostics;

namespace MyPdf.Controls
{
    internal class PdfHostTabItem : ChromeTabItem
    {
        PdfJsHost pdfViewer;
        public string _filePath;

        public PdfHostTabItem(string filePath, int? pageNumber)
        {
            _filePath = filePath;

            // add pdfjshost as content
            Dispatcher.InvokeAsync(new Action(() =>
            {
                pdfViewer = new PdfJsHost(filePath, pageNumber);

                pdfViewer.IsSelected = this.IsSelected;
                pdfViewer.SetBinding(PdfJsHost.IsSelectedProperty, new Binding(nameof(IsSelected)) { Source = this, Mode = BindingMode.OneWay });

                pdfViewer.CoreWebView2InitializationCompleted += (s, e) =>
                    pdfViewer.CoreWebView2.WebMessageReceived += PdfViewer_WebMessageReceived;

                Content = pdfViewer;
            }));

            //set the header
            try { Header = Path.GetFileNameWithoutExtension(filePath); } catch { };
        }

        private void PdfViewer_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = JsonSerializer.Deserialize<Dictionary<string, string>>(e.WebMessageAsJson);

            if (message != null && message.TryGetValue("action", out var actionName))
            {
                // Use reflection to find and invoke the method by name
                var method = this. GetType().GetMethod(actionName, BindingFlags.NonPublic | BindingFlags.Instance);
                method?.Invoke(this, null); // Calls the method if it exists, passing no parameters
            }
            else
            {
                Debug.Print("No action defined!");
            }
        }

        void OpenFile()
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    var openFileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "PDF Files (*.pdf)|*.pdf",
                        //Title = "בחר קובץ"
                    };

                    if (openFileDialog.ShowDialog() == true)
                        OpenPdfFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));
        }

        public void OpenPdfFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                PdfHostTabItem newTabItem = new PdfHostTabItem(filePath, null);
                if (Parent is ChromeTabControl tabControl) tabControl.Add(newTabItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Content is PdfJsHost pdfHost) pdfHost.Release();
        }
    }
}
