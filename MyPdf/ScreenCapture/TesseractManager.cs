using Microsoft.Win32;
using MyPdf.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;

namespace MyPdf.ScreenCapture
{
    public static class TesseractManager
    {
        static string tessDataFolder {  get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"); }
        static string _savedLangFile = "selectedTessLang.txt";

        public static async Task<string> ExtractTextFromImage(MemoryStream imageStream)
        {
            try
            {
                string tessLang = await AssetsManager.GetAssetAsync(_savedLangFile);
                if (string.IsNullOrEmpty(tessLang)) tessLang = "heb+eng";

                // Use the existing MemoryStream with Tesseract
                imageStream.Seek(0, SeekOrigin.Begin); // Reset position
                using (var engine = new TesseractEngine(tessDataFolder, tessLang, EngineMode.Default))
                using (var img = Pix.LoadFromMemory(imageStream.ToArray()))
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
        }

        public static async Task ChooseTessLang()
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
                await AssetsManager.WriteAssetAsync(_savedLangFile, string.Join("+", selectedLanguages));
            }
        }
    }
}
