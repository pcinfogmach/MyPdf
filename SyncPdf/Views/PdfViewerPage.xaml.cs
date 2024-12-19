using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using SyncPdf.ViewModels;
namespace SyncPdf.Views
{
    public partial class PdfViewerPage : Page
    {
		public string themeName = App.Current.Properties["Theme"]?.ToString()!= null? App.Current.Properties["Theme"]?.ToString(): "Windows11Light";
        public PdfViewerPage(PdfViewerViewModel viewModel)
        {
            InitializeComponent();		
            DataContext = viewModel;
			String path = AppDomain.CurrentDomain.BaseDirectory;
            path = path + "Assets/PDF_Succinctly.pdf";
            pdfViewer.Load(path);
			SfSkinManager.SetTheme(this, new Theme(themeName));
        }	
    }
}
