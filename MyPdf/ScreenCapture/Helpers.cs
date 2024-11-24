using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenCapture
{
    public static class Helpers
    {
        public static void SaveStringAs(string textToSave)
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
                File.WriteAllText(saveFileDialog.FileName, textToSave);
                var info = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (info == "he") MessageBox.Show("הטקסט נשמר בהצלחה", "שמור", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                else MessageBox.Show("Text saved successfully", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
