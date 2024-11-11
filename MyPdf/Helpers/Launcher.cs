using System.IO;
using System.Windows.Controls;
using ChromeTabs;
using MyPdf.Controls;

namespace MyPdf.Helpers
{
    internal static class Launcher
    {
        public static void LaunchApp()
        {
            ChromeTabsWindow window = new ChromeTabsWindow();
            window.Show();
            window.ChromeTabControl.SelectionChanged += (s, e) => 
            {
                if (window.ChromeTabControl.Items.Count <= 0) ShowInstructions(window.ChromeTabControl);
            };
            ShowInstructions(window.ChromeTabControl);
        }


        public static void ShowInstructions(ChromeTabControl tabControl)
        {
            string instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "MyPdfInstructions.pdf");
            tabControl.Add(new PdfHostTabItem(instructionsPdfPath, "🛈"));
        }
    }
}
