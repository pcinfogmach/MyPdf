using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ChromeTabs;
using MyPdf.Controls;

namespace MyPdf.Helpers
{
    public class Initializer
    {
        ChromeTabsWindow window;
        private readonly string savedTabsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "savedTabs.json");

        public ChromeTabsWindow LaunchApp()
        {
            window = new ChromeTabsWindow();
            window.Show();

            LoadSavedTabs();

            window.ChromeTabControl.SelectionChanged += (s, e) => OpenInstructionsPdf();

            window.Closing += Window_Closing;
            return window;
        }

        void LoadSavedTabs()
        {
            if (File.Exists(savedTabsPath))
            {
                string jsonText = File.ReadAllText(savedTabsPath);
                var saveData = JsonSerializer.Deserialize<SavedTabData>(jsonText);

                foreach (var filePath in saveData.Tabs)
                {
                    window.ChromeTabControl.Items.Add(new PdfHostTabItem(filePath));
                }

                if (saveData.SelectedIndex >= 0 && saveData.SelectedIndex < window.ChromeTabControl.Items.Count)
                {
                    window.ChromeTabStrip.SelectedIndex = saveData.SelectedIndex;
                }
            }
            
            OpenInstructionsPdf();
        }

        class SavedTabData
        {
            public List<string> Tabs { get; set; } = new();
            public int SelectedIndex { get; set; }
        }

        void OpenInstructionsPdf()
        {
            if (window.ChromeTabControl.Items.Count < 1)
            {
                string instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "🛈.pdf");
                window.ChromeTabControl.Add(new PdfHostTabItem(instructionsPdfPath));
            }
        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var currentTabList = window.ChromeTabControl.Items.Cast<PdfHostTabItem>().Select(t => t._filePath).ToList();
            int selectedIndex = window.ChromeTabControl.SelectedIndex;

            // Create an object to hold the file paths and the current index
            var saveData = new
            {
                Tabs = currentTabList,
                SelectedIndex = selectedIndex
            };
            File.WriteAllText(savedTabsPath, JsonSerializer.Serialize(saveData));
        }
    }
}
