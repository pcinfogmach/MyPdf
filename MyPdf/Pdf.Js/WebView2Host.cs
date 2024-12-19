using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Diagnostics;

namespace MyPdf.Pdf.Js
{
    public class WebView2Host : WebView2
    {
        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(WebView2Host), new PropertyMetadata(true, OnIsSelectedChanged));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as WebView2Host;
            control?.UpdateState();
        }

        private async void UpdateState()
        {
            if (!IsSelected)
            {
                this.Visibility = Visibility.Hidden;
                if (!this.IsVisible && this.CoreWebView2 != null)
                {
                    Debug.WriteLine(await this.CoreWebView2.TrySuspendAsync());
                }
            }
            else
            {
                this.Visibility = Visibility.Visible;
                if (this.CoreWebView2 != null)  this.CoreWebView2.Resume();
            }
        }
    }
}
