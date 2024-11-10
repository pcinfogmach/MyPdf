using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

namespace MyPdf.Helpers
{
    internal static class Test
    {
        public static void Launch(ChromeTabs.ChromeTabsWindow window)
        {
            string pdfViewerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfJs");

            WebView2 webView2 = new WebView2();
            webView2.EnsureCoreWebView2Async();
            webView2.CoreWebView2InitializationCompleted += (s, e) =>
            {
                webView2.CoreWebView2.SetVirtualHostNameToFolderMapping("pdfjs", pdfViewerPath, CoreWebView2HostResourceAccessKind.DenyCors);
                webView2.Source = new Uri($"https://pdfjs/web/viewer.html?file={Uri.EscapeDataString("Instructions.pdf")}");
            };

            window.ChromeTabControl.Items.Add(new TabItem
            {
                Content = webView2,
                Header = "🛈" // Added a header for the tab
            });

        }
    }
}
