using iText.IO.Image;
using iText.Kernel.Pdf;
using PDFIndexer.Base;
using PDFIndexer.CommomModels;
using PDFIndexer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace PDFIndexer.Utils
{
    static class ProcessResult
    {
        private static string rawTempPath = Path.Combine(Directory.GetCurrentDirectory(), "_temp");
        private static string tempPath = rawTempPath + @"\out.pdf";
        private static string gsPath = Path.Combine(Environment.CurrentDirectory,@"Ghost\gswin64c.exe");




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
                    ImageUri = ConvertPdf2Image(ExtracPage(ho))
                };
                //so.Sample = CutImage(so.ImageUri);
                so.Sample = "";
                objects.Add(so);
            }
            return objects;
        }

        private static string ExtracPage(HighlightObject result)
        {
            var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(result.Metadata.PDFURI));
            PdfPage origPage = pdfInput.GetPage(result.PageNumber);

            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(tempPath)))
            {
                pdfInput.CopyPagesTo(1, 1, pdfOutput);
            }
            return tempPath;
        }

        private static string ConvertPdf2Image(string readerDoc)
        {
            PdfImageConverter imageConverter = new PdfImageConverter(gsPath, rawTempPath, "204.8");
            Stream[] pdfPageImageList = null;

            using (var pdfInput = File.OpenRead(readerDoc))
            {
                try
                {
                    //The array of streams will respect the page number-1, page 1 equal index 0;
                    imageConverter.GenerateImage(pdfInput, ref pdfPageImageList);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error generating pdf images {ex.Message}");
                }
            }

            FileInfo f = new FileInfo(readerDoc);

            return ImageProcessing.UploadImages(pdfPageImageList[0], f.Name.Replace(".pdf", ""));
        }   

        private static Stream CutImage(Stream _oldImage, float X, float Y, float W, float H)
        {
            return ImageProcessing.Crop(_oldImage, X, Y, W, H);
        }

        private static void SaveImage(byte[] _oldImage)
        {
            var fs = new BinaryWriter(new FileStream(rawTempPath + "\\imgout.jpg", FileMode.Append, FileAccess.Write));
            fs.Write(_oldImage);
            fs.Close();
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

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
