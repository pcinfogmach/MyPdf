using MyPdf.Assets.Locale;
using MyPdf.ScreenCapture;
using PdfiumViewer;
using PdfToolsLib;
using PdfToolsLib.TextExtractor;
using System.IO;
using System.Text;

namespace MyPdf.TextExtractor
{
    public class OcrExtractor : TextExtractorBase
    {
        public async Task ExtractTextFromWholeDocument(string pdfPath)
        {
            if (!File.Exists(pdfPath))
            {
                LocaleHelper.LocalizedErrorMessage("InvalidPath");
                return;
            }

            var stb = new StringBuilder();
            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    stb.AppendLine(await PerformOcr(pdfDocument, i) + "\n\n");
                }
            }
            await TextSave.SaveAndShow(stb.ToString().Trim());
        }

        public async Task ExtractTextFromSpecificPage(string pdfPath, int pageNumber)
        {
            if (!File.Exists(pdfPath))
            {
                LocaleHelper.LocalizedErrorMessage("InvalidPath");
                return;
            }

            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                if (pageNumber < 1 || pageNumber > pdfDocument.PageCount)
                    return;

                await TextSave.SaveAndShow(await PerformOcr(pdfDocument, pageNumber - 1));
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

                var stringBuilder = new StringBuilder();

                foreach (var (start, end) in rangeList)
                {
                    for (int i = start - 1; i < end; i++)
                    {
                        stringBuilder.AppendLine(await PerformOcr(pdfDocument, i - 1) + "\n\n");
                    }

                }
                await TextSave.SaveAndShow(stringBuilder.ToString());
            }
        }


        async Task<string> PerformOcr(PdfDocument pdfDocument, int pagNumber)
        {
            try
            {
                using (var pageImage = pdfDocument.Render(pagNumber, 300, 300, PdfRenderFlags.CorrectFromDpi))
                using (var memoryStream = new MemoryStream())
                {
                    pageImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return await TesseractManager.ExtractTextFromImage(memoryStream);
                }

            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
    }
}
