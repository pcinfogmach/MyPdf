using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using ChromeTabs;
using MyPdf.Assets;
using MyPdf.Controls;
using MyPdf.HistoryAndUserTags;
using Pdf.Js;
using PdfToolsLib;

namespace MyPdf
{
    public class MainWindow : ChromeTabsWindow
    {
        private readonly string savedTabsPath = "savedChromeTabs.json";

        public MainWindow()
        {
            ChromeTabControl.TabAdded += ChromeTabControl_TabAdded;
            HistoryTree.SelectedItemChanged += HistoryTree_SelectedItemChanged;
            Closing += Window_Closing;
        }

        private void HistoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is HistoryItem historyItem)
            {
                ChromeTabControl.Add(new PdfHostTabItem(historyItem.Path, null));
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

        public override async void LoadSavedTabs()
        {
            string jsonText = await AssetsManager.GetAssetAsync(savedTabsPath);
            if (string.IsNullOrEmpty(jsonText)) return;

            var saveData = JsonSerializer.Deserialize<SavedTabData>(jsonText);

            foreach (var filePath in saveData.Tabs)
            {
                ChromeTabControl.Items.Add(new PdfHostTabItem(filePath, null));
            }

            await Task.Delay(500); // Delay for 500 milliseconds
            if (saveData.SelectedIndex >= 0 && saveData.SelectedIndex < ChromeTabControl.Items.Count)
            {               
                ChromeTabControl.SelectedIndex = saveData.SelectedIndex;
            }
            else if (ChromeTabControl.Items.Count == 1) 
            {
                ChromeTabControl.SelectedIndex = 0; 
            }
        }


        class SavedTabData
        {
            public List<string> Tabs { get; set; } = new();
            public int SelectedIndex { get; set; }
        }

        private async void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var currentTabList = ChromeTabControl.Items.Cast<PdfHostTabItem>().Select(t => t._filePath).ToList();
            int selectedIndex = ChromeTabControl.SelectedIndex;

            // Create an object to hold the file paths and the current index
            var saveData = new
            {
                Tabs = currentTabList,
                SelectedIndex = selectedIndex
            };
            await AssetsManager.WriteAssetAsync(savedTabsPath, JsonSerializer.Serialize(saveData));
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
                        try { ChromeTabControl.Add(new PdfHostTabItem(openFileDialog.FileName, null)); }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));
        }

        public override void PrintWithEdge() { if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost) pdfJsHost.Print(); }

        public async override void ExtractText()
        {
          
            if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost)
            {
                int pageNumber = await pdfJsHost.GetCurrentPageNumber();
                new TextExtractDialog(pdfJsHost._sourceFilePath, pageNumber).ShowDialog();
            }
        }

        public override void ShowHelp()
        {
            string instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Instructions - MyPdf.pdf");
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he")
                instructionsPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "הוראות - MyPdf.pdf");

            if (File.Exists(instructionsPdfPath)) ChromeTabControl.Add(new PdfHostTabItem(instructionsPdfPath, null));
        }

        public override void SaveFile() { if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost) pdfJsHost.SaveFile(); }
        public override void SaveFileAS() { if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost) pdfJsHost.SaveAs(); }

        public override async void CreateUserTag()
        {
            if (ChromeTabControl.SelectedContent is PdfJsHost pdfJsHost)
            {
                string currentPageQuery = await pdfJsHost.ExecuteScriptAsync("PDFViewerApplication.pdfViewer.currentPageNumber");
                int.TryParse(currentPageQuery, out int currentPageNum);
                var inputBox = new UserTagInputBox(pdfJsHost._sourceFilePath, currentPageNum);
                if (inputBox.ShowDialog() == true)
                {
                    var userInput = inputBox.Result;
                    if (userInput != null)
                    {
                        if (UserTagTreeView.SelectedItem is UserTagGroup userTagGroup) { userTagGroup.Add(userInput); userTagGroup.IsExpanded = true; }
                        else UserTagManager.AddTag(userInput);
                    }                  
                }
            }
        }
        public override void EditUserTag()
        {
            if (UserTagTreeView.SelectedItem is UserTagItem userTagItem)
            {
                var inputBox = new UserTagInputBox(userTagItem.Path, userTagItem.Position);
                if (inputBox.ShowDialog() == true)
                {
                    var userInput = inputBox.Result;
                    if (userInput != null)
                    {
                        userTagItem.Name = userInput.Name;
                        userTagItem.Position = userInput.Position;
                    }
                }
            }
        }

        public override void OpenSelectedUserTag()
        {
            if (UserTagTreeView.SelectedItem is UserTagItem userTagItem)
            {
                ChromeTabControl.Add(new PdfHostTabItem(userTagItem.Path, userTagItem.Position));
            }
        }
    }
}
