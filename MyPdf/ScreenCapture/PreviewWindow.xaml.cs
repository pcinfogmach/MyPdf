using Microsoft.Win32;
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
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); e.Handled = true; }
        }

        private async void ExtractTextFromImage()
        {
            ExtractedTextBox.Text = await Task.Run(() =>
            {
                try
                {
                    string tessDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCapture", "tessdata");
                    List<string> files = Directory.GetFiles(tessDataFolder, "*.traineddata").ToList();
                    string tessLang = Path.GetFileNameWithoutExtension(files[0]);
                    if (files.Count > 1)
                    {
                        for (int i = 1; i < files.Count; i++) { tessLang += "+" + Path.GetFileNameWithoutExtension(files[i]); }
                    }

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
                    return $"Failed to extract text: {ex.Message}";
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
                MessageBox.Show("Screenshot saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"ExtractedText_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                Filter = "Text File (*.txt)|*.txt|" +
              "Word Documents (*.docx;*.doc;*.rtf;)|*.docx;*.doc;*.rtf;|" +
              "Html (*.html)|*.html|" +
              "OpenDocument Formats (*.odt;*.ods)|*.odt;*.ods|" +
              "All Files (*.*)|*.*"
            };


            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, ExtractedTextBox.Text);
                Close();
                MessageBox.Show("Text saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CopyImageButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(_bitmapImage);
            Close();
            MessageBox.Show("Image copied to clipboard!", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CopyTextButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ExtractedTextBox.Text);
            Close();
            MessageBox.Show("Text copied to clipboard!", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
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

            captureWindow.Loaded += (s, e) => {  this.Close(); }; 
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
    }
}
