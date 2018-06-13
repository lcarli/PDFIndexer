using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using PDFIndexer.Base;
using PDFIndexer.CommomModels;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using iText.Layout.Element;
using Leadtools.Codecs;
using Leadtools;

namespace PDFIndexer.Utils
{
    static class ProcessResult
    {
        private static string rawTempPath = Path.Combine(Directory.GetCurrentDirectory(), "_temp");
        private static string tempPath = rawTempPath+"\\out.pdf";
        public static List<SampleObject> ProcessResults(IEnumerable<IndexMetadata> results, string keyword)
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

            //Delete pos process
            //DeleteFile(tempPath);
           
            return list;
        }

        private static void DeleteFile(string path)
        {
            File.Delete(path);
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

        private static PdfDocument ExtracPage(HighlightObject result)
        {
            var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(result.Metadata.PDFURI));
            PdfPage origPage = pdfInput.GetPage(result.PageNumber);
            //PdfDocument pdf = new PdfDocument(new PdfWriter(tempPath));
            //var t = origPage.CopyTo(pdf);

            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(tempPath)))
            {
                pdfInput.CopyPagesTo(1,1, pdfOutput);
                return pdfOutput;
            }
        }

        private static byte[] ConvertPdf2Image(PdfDocument page)
        {
            RasterCodecs _codecs = new RasterCodecs();
            RasterImage _Image = _codecs.Load(tempPath);
            _codecs.Save(_Image, tempPath+"\\image.png", RasterImageFormat.Png, 24);


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
