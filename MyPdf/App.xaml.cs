using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using MyPdf.Helpers;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace MyPdf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ErrorLog.Initialize();
            Launcher.LaunchApp();
            Task.Run(() => {UpdateChecker.CheckForUpdates();});
        }

       
    }
}
