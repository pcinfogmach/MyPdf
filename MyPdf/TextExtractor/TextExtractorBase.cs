using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PdfToolsLib.TextExtractor
{
    public class TextExtractorBase
    {
        public bool ValidateInputs(string pdfPath, string ranges, out string message)
        {
            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
            {
                message = "PDF file not found.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                message = "Page ranges cannot be null or empty.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public List<(int Start, int End)> ParseRanges(string ranges, int maxPage)
        {
            try
            {
                return ranges.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(part => part.Split('-'))
                             .Select(bounds => bounds.Length == 1
                                ? (Start: int.Parse(bounds[0]), End: int.Parse(bounds[0]))
                                : (Start: int.Parse(bounds[0]), End: int.Parse(bounds[1])))
                             .Where(r => r.Start >= 1 && r.End >= r.Start && r.End <= maxPage)
                             .OrderBy(r => r.Start)
                             .ToList();
            }
            catch
            {
                ShowMessage("Invalid range format.");
                return new List<(int, int)>();
            }
        }

        public void ShowMessage(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}


//pdfJsHost.GetTextFromCurrentPageAsync();
//MessageBoxResult result;

//if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "he")
//{
//    string message = "האם אתה רוצה לחלץ טקסט מכל המסמך? לחץ 'לא' כדי לחלץ מהעמוד הנוכחי.\nשים לב! חילוץ טקסט עלול לקחת זמן מה תלוי במשקל המסמך. התוכנה תחלץ את הטקסט ברקע ותודיע לכם בגמר הפעולה.";
//    result = MessageBox.Show(message, "בחירת אופן החילוץ", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RightAlign |MessageBoxOptions.RtlReading);
//}
//else
//{
//    string message = "Do you want to extract text from the whole document? Click 'No' to extract from the current page.\nPlease Note! extracting text can take a while depending on your file. The program will extract the text in the bakground and notify you when done.";
//    result = MessageBox.Show(message, "Choose Extraction Method", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
//}

//await Task.Run(async () =>
//{
//    if (result == MessageBoxResult.Yes) await PdfToolsLib.TextExtractor.ExtractTextFromWholeDocument(pdfJsHost._sourceFilePath);
//    else if (result == MessageBoxResult.No)
//    {
//        int pageNumber = await pdfJsHost.GetCurrentPageNumber();
//        await PdfToolsLib.TextExtractor.ExtractTextFromSpecificPage(pdfJsHost._sourceFilePath, pageNumber);
//    }
//});
