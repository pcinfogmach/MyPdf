using Docnet.Core.Models;
using Docnet.Core;
using PdfiumViewer;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using PdfToolsLib.TextExtractor;

namespace PdfToolsLib
{
    public class PdfiumViewerTextExtractor : TextExtractorBase
    {
        public async Task ExtractTextFromWholeDocument(string pdfPath)
        {
            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
                return;

            var stb = new StringBuilder();
            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    stb.AppendLine(pdfDocument.GetPdfText(i) + "\n\n");
                }
            }

            await TextSave.SaveAndShow(stb.ToString().Trim());
        }

        public async Task ExtractTextFromSpecificPage(string pdfPath, int pageNumber)
        {
            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
                return;

            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                if (pageNumber < 1 || pageNumber > pdfDocument.PageCount)
                    return;

                await TextSave.SaveAndShow(pdfDocument.GetPdfText(pageNumber - 1));
            }
        }

        public async Task ExtractTextFromPageRanges(string pdfPath, string ranges)
        {
            if (!ValidateInputs(pdfPath, ranges, out string validationMessage))
            {
                ShowMessage(validationMessage);
                return;
            }

            try
            {
                using (var pdfDocument = PdfDocument.Load(pdfPath))
                { 
                    var pageCount = pdfDocument.PageCount;
                    var rangeList = ParseRanges(ranges, pageCount);

                var stringBuilder = new StringBuilder();
                foreach (var (start, end) in rangeList)
                    for (int i = start - 1; i < end; i++)
                            stringBuilder.AppendLine((pdfDocument.GetPdfText(i - 1) + "\n\n"));

                await TextSave.SaveAndShow(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred: {ex.Message}");
            }
        }
    }
}
