using Microsoft.Win32;
using MyPdf.Assets;
using ScreenCapture;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tesseract;
using ThemedWindow.Controls;

namespace ScreenCaptureLib
{
    public partial class PreviewWindow : TWindow
    {
        private readonly BitmapImage _bitmapImage;
        private readonly MemoryStream _imageStream;
        string tessDataFolder;
        string savedLangFile = "selectedTessLang.txt";

        public PreviewWindow(BitmapImage bitmapImage, MemoryStream imageStream)
        {
            tessDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

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
            ExtractedTextBox.Text = await Task.Run(async () =>
            {
                try
                {
                    string tessLang = await AssetsManager.GetAssetAsync(savedLangFile);
                    if (string.IsNullOrEmpty(tessLang)) tessLang = "heb+eng";                   

                    // Use the existing MemoryStream with Tesseract
                    _imageStream.Seek(0, SeekOrigin.Begin); // Reset position
                    using (var engine = new TesseractEngine(tessDataFolder, tessLang, EngineMode.Default))
                    using (var img = Pix.LoadFromMemory(_imageStream.ToArray()))
                    using (var page = engine.Process(img))
                    {
                        // Get the extracted text
                        var text = page.GetText().Trim();

                        // Replace single newlines with spaces and keep paragraph breaks
                        text = Regex.Replace(text, @"(?<!\n)\n(?!\n)", " ");
                        text = Regex.Replace(text, @"\n+", "\n");
                        return text;
                    }
                }
                catch (Exception ex)
                {
                    string message = $"[{DateTime.Now}] An error occurred:\n" +
                                  $"Message: {ex.Message}\n" +
                                  $"Source: {ex.Source}\n" +
                                  $"Stack Trace: {ex.StackTrace}\n" +
                                  $"Inner Exception: {ex.InnerException?.Message ?? "None"}\n" +
                                  $"Target Site: {ex.TargetSite}\n";

                    return $"Failed to extract text: {message}";
                }
            });
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

        private void ChooseOcrLanguageButton_Click(object sender, RoutedEventArgs e) =>  ChooseTessLang();

        async void ChooseTessLang()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Tesseract Trained Data Files (*.traineddata)|*.traineddata",
                InitialDirectory = tessDataFolder
            };

            if (dialog.ShowDialog() == true)
            {
                List<string> selectedLanguages = dialog.FileNames
                                                      .Select(file => Path.GetFileNameWithoutExtension(file))
                                                      .ToList();

                // Save the selected languages to disk (for future use)
                await AssetsManager.WriteAssetAsync(savedLangFile, string.Join("+", selectedLanguages));
            }
        }
    }
}