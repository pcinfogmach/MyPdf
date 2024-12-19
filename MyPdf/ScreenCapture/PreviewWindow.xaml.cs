using Microsoft.Win32;
using MyPdf.Assets;
using MyPdf.ScreenCapture;
using ScreenCapture;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ThemedWindow.Controls;

namespace ScreenCaptureLib
{
    public partial class PreviewWindow : TWindow
    {
        private readonly BitmapImage _bitmapImage;
        private readonly MemoryStream _imageStream;

        public PreviewWindow(BitmapImage bitmapImage, MemoryStream imageStream)
        {
            InitializeComponent();
            SetButtonContentBasedOnLanguage();
            _bitmapImage = bitmapImage;
            _imageStream = imageStream;
            PreviewImage.Source = _bitmapImage;
            ExtractTextFromImage();
        }

        private void SetButtonContentBasedOnLanguage()
        {
            var info = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            // Check if the system language is set to Hebrew
            if (info == "he")
            {
                this.Title = "צילום מסך";
                this.FlowDirection = FlowDirection.RightToLeft;
                ExtractedTextBox.Text = "מחלץ טקסט. אנא המתן...";
                // Change button content to Hebrew
                SaveImageButton.ToolTip = "שמור תמונה";
                SaveTextButton.ToolTip = "שמור טקסט";
                CopyImageButton.ToolTip = "העתק תמונה";
                CopyTextButton.ToolTip = "העתק טקסט";
                RestartButton.ToolTip = "לכידה חדשה";
                GoogleTranslateButton.ToolTip = "תרגום גוגל";
                EditImageButton.ToolTip = "ערוך תמונה";
                ChooseOcrLanguageButton.ToolTip = "בחר שפות עבור חילוץ טקסט (ברירת המחדל הינה עברית + אנגלית) לקבלת תוצאות משופרות בחר שפה אחת בלבד";
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); e.Handled = true; }
        }

        private async void ExtractTextFromImage()
        {
            await Dispatcher.InvokeAsync(new Action( async() =>
            {
                var result = await TesseractManager.ExtractTextFromImage(_imageStream);
                var hebrewMatches = Regex.Matches(result, @"\p{IsHebrew}");
                ExtractedTextBox.FlowDirection = hebrewMatches.Count() > (ExtractedTextBox.Text.Length / 2) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                ExtractedTextBox.Text = result;
            }));
        }


        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png",
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(_bitmapImage));
                    encoder.Save(fileStream);
                }
                Close();
                var info = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (info == "he") MessageBox.Show("הצילום מסך נשמר בהצלחה", "שמור", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                else MessageBox.Show("Screenshot saved successfully", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.SaveStringAs(ExtractedTextBox.Text);
            Close();
        }

        private void CopyImageButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(_bitmapImage);
            Close();
            var info = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (info == "he") MessageBox.Show("התמונה הועתקה בהצלחה", "העתק", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            else MessageBox.Show("Image copied to clipboard", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CopyTextButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ExtractedTextBox.Text);
            Close();
            var info = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (info == "he") MessageBox.Show("הטקסט הועתק בהצלחה", "העתק", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            else MessageBox.Show("Text copied to clipboard", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Window captureWindow;
            if (this.Owner != null)
            {
                captureWindow = new ScreenCaptureWindow(false)
                {
                    WindowState = this.Owner.WindowState,
                    Height = this.Owner.ActualHeight,
                    Width = this.Owner.ActualWidth,
                    Left = this.Owner.Left,
                    Top = this.Owner.Top,
                    Owner = this.Owner,
                };
            }
            else
            {
                captureWindow = new ScreenCaptureWindow();
            }

            captureWindow.Loaded += (s, e) => { this.Close(); };
            captureWindow.Show();
        }

        private void GoogleTranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string textToTranslate = ExtractedTextBox.Text;
            string targetLanguage = Regex.Match(textToTranslate, @"\p{IsHebrew}").Success ? "en" : "he";
            string translateUrl = $"https://translate.google.com/?sl=auto&tl={targetLanguage}&text={Uri.EscapeDataString(textToTranslate)}&op=translate";
            Process.Start(new ProcessStartInfo(translateUrl) { UseShellExecute = true });
            this.Close();
        }

        private void EditImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Generate a temporary file path
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_image.png");

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_bitmapImage));

            using FileStream fileStream = new FileStream(tempFilePath, FileMode.Create);
            encoder.Save(fileStream);

            // Start Paint with the image file path
            using Process process = new Process();
            process.StartInfo.FileName = "mspaint";
            process.StartInfo.ArgumentList.Add(tempFilePath);
            process.Start();
        }

        private async void ChooseOcrLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            await TesseractManager.ChooseTessLang();
            ExtractTextFromImage(); 
        }
    }
}