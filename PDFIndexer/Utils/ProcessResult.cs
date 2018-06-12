using iText.Kernel.Pdf;
using PDFIndexer.Base;
using PDFIndexer.CommomModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer.Utils
{
    static class ProcessResult
    {
        private static string tempPath = "";
        public static List<SampleObject> ProcessResults(List<IndexMetadata> results, string keyword)
        {
            List<SampleObject> list = new List<SampleObject>();

            foreach (var result in results)
            {
                var temp = CreateSampleObject(result, keyword);
                foreach (var item in temp)
                {
                    list.Add(item);
                }
            }
            File.Delete(tempPath);
            return list;
        }


        //TODO: if there is more than one word in the same cutImage, create only one object
        private static List<SampleObject> CreateSampleObject(IndexMetadata result, string keyword)
        {
            List<SampleObject> objects = new List<SampleObject>();

            //Get all Highlighted Object per page
            List<HighlightObject> hos = HightlightWords(result, keyword);

            //Extract page and process image to construct SampleObject
            foreach (var ho in hos)
            {
                SampleObject so = new SampleObject
                {
                    Metadata = result,
                    Image = ConvertPdf2Image(ExtracPage(ho))
                };
                so.Sample = CutImage(so.Image);
                objects.Add(so);
            }
            return objects;
        }

        private static string ExtracPage(HighlightObject result)
        {
            File.Delete(tempPath);
            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(result.Metadata.PDFURI)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(tempPath)))
            {
                pdfInput.CopyPagesTo(result.PageNumber, result.PageNumber, pdfOutput);
                return tempPath;
            }
        }

        private static byte[] ConvertPdf2Image(string pagePath)
        {
            return null;
        }

        private static byte[] CutImage(byte[] _oldImage)
        {
            return null;
        }


        private static List<HighlightObject> HightlightWords(IndexMetadata input, string keyword)
        {
            List<PdfMetadata> words = new List<PdfMetadata>();
            List<List<PdfMetadata>> wordPerPage = new List<List<PdfMetadata>>();
            List<HighlightObject> list = new List<HighlightObject>();
            int LastPage = 1;

            foreach (var word in input.ListOfWords)
            {
                if (keyword == word.Text)
                {
                    if (word.page == LastPage)
                    {
                        words.Add(word);
                    }
                    else
                    {
                        LastPage = word.page;
                        wordPerPage.Add(words);
                        words.Clear();
                        words.Add(word);
                    }
                }
            }

            foreach (var item in wordPerPage)
            {
                list.Add(new HighlightObject
                {
                    Metadata = input,
                    HighlightedWords = ConvertWord2BoundingBox(item),
                    Keyword = keyword,
                    PageNumber = item[0].page
                });
            }

            return list;
        }

        private static List<BoundingBox> ConvertWord2BoundingBox(List<PdfMetadata> words)
        {
            List<BoundingBox> list = new List<BoundingBox>();

            foreach (var word in words)
            {
                list.Add(new BoundingBox { X = word.X, Y = word.Y, Height = word.Height, Width = word.Width });
            }

            return list;
        }

    }
}
