using MyPdf.TextExtractor;
using System.Globalization;
using System.Windows;

namespace PdfToolsLib
{
    /// <summary>
    /// Interaction logic for TextExtractDialog.xaml
    /// </summary>
    public partial class TextExtractDialog : ThemedWindow.Controls.ThemedToolWindow
    {
        string _filePath;

        public TextExtractDialog(string filePath, int pageNumber)
        {
            InitializeComponent();

            _filePath = filePath;

            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he")
            {
                InstructionsTextBlock.Text = "עמוד מסויים: 1 || טווח עמודים: 1-2,25-16 || השאר ריק לחילוץ מכל המסמך";
                ExtractButtonTextBlock.Text = "חלץ";
                OcrCheckBox.Content = "השתמש בזיהוי תווים (Ocr)";
            }

            PageEnumTextBox.Text = pageNumber.ToString();
            PageEnumTextBox.SelectAll();
            PageEnumTextBox.Focus();
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e) => ExtractText();

        private void PageEnumTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) 
        {
            if (e.Key == System.Windows.Input.Key.Enter) ExtractText();
        }

        async void ExtractText()
        {
            Close();
            try
            {
                string pageEnum = PageEnumTextBox.Text.Trim();
                if (OcrCheckBox.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(pageEnum)) await new OcrExtractor().ExtractTextFromWholeDocument(_filePath);
                    else if (pageEnum.Contains("-") || pageEnum.Contains(",")) await new OcrExtractor().ExtractTextFromPageRanges(_filePath, pageEnum);
                    else if (int.TryParse(pageEnum, out int resultNumber)) await new OcrExtractor().ExtractTextFromSpecificPage(_filePath, resultNumber);
                }
                else
                {
                    if (string.IsNullOrEmpty(pageEnum)) await new PdfiumViewerTextExtractor().ExtractTextFromWholeDocument(_filePath);
                    else if (pageEnum.Contains("-") || pageEnum.Contains(",")) await new PdfiumViewerTextExtractor().ExtractTextFromPageRanges(_filePath, pageEnum);
                    else if (int.TryParse(pageEnum, out int resultNumber)) await new PdfiumViewerTextExtractor().ExtractTextFromSpecificPage(_filePath, resultNumber);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }            
        }

        private void ThemedToolWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape) Close();
        }
    }
}
