using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using ChromeTabs;
using MyPdf.Controls;
using MyPdf.HistoryAndUserTags;
using Pdf.Js;

namespace MyPdf
{
    public class MainWindow : ChromeTabsWindow
    {
        private readonly string savedTabsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "savedTabs.json");

        public MainWindow()
        {
            ChromeTabControl.TabAdded += ChromeTabControl_TabAdded;
            HistoryTree.SelectedItemChanged += HistoryTree_SelectedItemChanged;
            this.Closing += Window_Closing;
        }

        private void HistoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is HistoryItem historyItem)
            {
                this.ChromeTabControl.Add(new PdfHostTabItem(historyItem.Path));
            }
        }

        private void ChromeTabControl_TabAdded(object? sender, TabItemEventArgs e)
        {
            if (e.TabItem is PdfHostTabItem tabItem)
            {
                string fileName = Path.GetFileName(tabItem._filePath);
                HistoryLogger.AddHistoryItem(fileName, tabItem._filePath);
            }
        }

        public void LoadSavedTabs()
        {
            if (File.Exists(savedTabsPath))
            {
                string jsonText = File.ReadAllText(savedTabsPath);
                var saveData = JsonSerializer.Deserialize<SavedTabData>(jsonText);

                foreach (var filePath in saveData.Tabs)
                {
                    this.ChromeTabControl.Items.Add(new PdfHostTabItem(filePath));
                }

                if (saveData.SelectedIndex >= 0 && saveData.SelectedIndex < this.ChromeTabControl.Items.Count)
                {
                    this.ChromeTabStrip.SelectedIndex = saveData.SelectedIndex;
                }
            }
        }
        class SavedTabData
        {
            public List<string> Tabs { get; set; } = new();
            public int SelectedIndex { get; set; }
        }
        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var currentTabList = this.ChromeTabControl.Items.Cast<PdfHostTabItem>().Select(t => t._filePath).ToList();
            int selectedIndex = this.ChromeTabControl.SelectedIndex;

            // Create an object to hold the file paths and the current index
            var saveData = new
            {
                Tabs = currentTabList,
                SelectedIndex = selectedIndex
            };
            File.WriteAllText(savedTabsPath, JsonSerializer.Serialize(saveData));
        }
    

        public override void OpenFile()
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
                    {
                        try { ChromeTabControl.Add(new PdfHostTabItem(openFileDialog.FileName)); }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                base.OpenFile();
            }));
        }
        public override void ShowHelp()
        {
            string instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyPdf", "Assets", "Instructions - MyPdf.pdf");
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he")
                instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyPdf", "Assets", "הוראות - MyPdf.pdf");

            if (File.Exists(instructionsPdfPath)) this.ChromeTabControl.Add(new PdfHostTabItem(instructionsPdfPath));
            base.ShowHelp();
        }
        public override void SaveFile() { if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost) pdfJsHost.SaveFile();}
        public override void SaveFileAS() { if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost) pdfJsHost.SaveAs(); }
    }
}
