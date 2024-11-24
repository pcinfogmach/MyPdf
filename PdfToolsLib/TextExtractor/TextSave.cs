using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PdfToolsLib
{
    public static class TextSave
    {
        public async static Task SaveAndShow(string text)
        {
            try
            {
                // Create a temporary file
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.txt");

                // Save the text to the file
                await File.WriteAllTextAsync(tempFilePath, text, Encoding.UTF8);

                // Open the file in the default text viewer
                Process.Start(new ProcessStartInfo
                {
                    FileName = tempFilePath,
                    UseShellExecute = true // Use shell to open with the default app
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
