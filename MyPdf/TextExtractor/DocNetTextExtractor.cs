//using Docnet.Core.Models;
//using Docnet.Core;
//using System;
//using System.IO;
//using System.Text;
//using System.Windows;
//using PdfToolsLib.TextExtractor;

//namespace PdfToolsLib
//{
//    public class DocNetTextExtractor : TextExtractorBase
//    {
//        public async Task ExtractTextFromPage(string pdfPath, int pageNumber)
//        {
//            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
//                throw new FileNotFoundException("PDF file not found.");

//            using (var library = DocLib.Instance)
//            using (var docReader = library.GetDocReader(pdfPath, new PageDimensions(1080, 1920)))
//            {
//                if (pageNumber < 1 || pageNumber > docReader.GetPageCount())
//                {
//                    MessageBox.Show($"{pageNumber}, Invalid page number.");
//                    return;
//                }

//                using (var pageReader = docReader.GetPageReader(pageNumber - 1))
//                {
//                    var rawText = pageReader.GetText();
//                    await TextSave.SaveAndShow(rawText);
//                }
//            }
//        }

//        public async Task ExtractTextFromWholeDocument(string pdfPath)
//        {
//            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
//            {
//                MessageBox.Show("PDF file not found.");
//                return;
//            }

//            using (var library = DocLib.Instance)
//            using (var docReader = library.GetDocReader(pdfPath, new PageDimensions(1080, 1920)))
//            {
//                var pageCount = docReader.GetPageCount();
//                var stringBuilder = new StringBuilder();

//                for (int i = 0; i < pageCount; i++)
//                {
//                    using (var pageReader = docReader.GetPageReader(i))
//                    {
//                        stringBuilder.AppendLine(pageReader.GetText() + "\n\n");
//                    }
//                }

//                await TextSave.SaveAndShow(stringBuilder.ToString());
//            }
//        }

//        public async Task ExtractTextFromPageRanges(string pdfPath, string ranges)
//        {
//            if (!ValidateInputs(pdfPath, ranges, out string validationMessage))
//            {
//                ShowMessage(validationMessage);
//                return;
//            }
//            using var library = DocLib.Instance;
//            using var docReader = library.GetDocReader(pdfPath, new PageDimensions(1080, 1920));
//            var pageCount = docReader.GetPageCount();
//            var rangeList = ParseRanges(ranges, pageCount);

//            var stringBuilder = new StringBuilder();
//            foreach (var (start, end) in rangeList)
//                for (int i = start - 1; i < end; i++)
//                    using (var pageReader = docReader.GetPageReader(i))
//                        stringBuilder.AppendLine(pageReader.GetText() + "\n\n");

//            await TextSave.SaveAndShow(stringBuilder.ToString());
//        }
//    }
//}
