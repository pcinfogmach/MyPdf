using PdfiumViewer;
using System.IO;
using System.Text;
using System.Windows;
using PdfToolsLib.TextExtractor;
using MyPdf.TextExtractor;
using MyPdf.Assets.Locale;

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

            string result = stb.ToString().Trim();
            if (string.IsNullOrEmpty(result))
            {
                if (LocaleHelper.LocalizedYesNoMessage("useOcr", MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    await new OcrExtractor().ExtractTextFromWholeDocument(pdfPath);
                    return;
                }
            }
            await TextSave.SaveAndShow(result);
        }

        public async Task ExtractTextFromSpecificPage(string pdfPath, int pageNumber)
        {
            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
                return;

            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                if (pageNumber < 1 || pageNumber > pdfDocument.PageCount)
                    return;

                string result = pdfDocument.GetPdfText(pageNumber - 1);
                if (string.IsNullOrEmpty(result))
                {
                    if (LocaleHelper.LocalizedYesNoMessage("useOcr", MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        await new OcrExtractor().ExtractTextFromSpecificPage(pdfPath, pageNumber);
                        return;
                    }
                }
                await TextSave.SaveAndShow(result);
            }
        }

        public async Task ExtractTextFromPageRanges(string pdfPath, string ranges)
        {
            if (!ValidateInputs(pdfPath, ranges, out string validationMessage))
            {
                ShowMessage(validationMessage);
                return;
            }

            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                var pageCount = pdfDocument.PageCount;
                List<(int start, int end)> rangeList = ParseRanges(ranges, pageCount);

                var stb = new StringBuilder();
                foreach (var (start, end) in rangeList)
                {
                    for (int i = start - 1; i < end; i++)
                    {
                        stb.AppendLine((pdfDocument.GetPdfText(i) + "\n\n"));
                    }

                }

                string result = stb.ToString().Trim();
                if (string.IsNullOrEmpty(result))
                {
                    if (LocaleHelper.LocalizedYesNoMessage("useOcr", MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        await new OcrExtractor().ExtractTextFromPageRanges(pdfPath, ranges);
                        return;
                    }
                }
                await TextSave.SaveAndShow(stb.ToString());
            }
        }
    }
}
