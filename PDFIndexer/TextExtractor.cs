using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Linq;
using iText.IO.Font;

namespace PDFIndexer
{
    public class TextExtractor
    { 
        public PdfMetadata Extract(string path, bool detailed = false)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(path));

            int countPages = pdfDoc.GetNumberOfPages();

            StringBuilder completeText = new StringBuilder();
            for (int i = 1; i < countPages + 1; i++)
            {
                PdfPage p = pdfDoc.GetPage(i);
                completeText.Append(PdfTextExtractor.GetTextFromPage(p));
            }

            if (detailed)
            {
                MyCustomListener listener = new MyCustomListener();
                var parser = new PdfCanvasProcessor(listener);
                parser.ProcessPageContent(pdfDoc.GetFirstPage());

                BlockPage bp = listener.GetResults();



                return new PdfMetadata
                {
                    Text = completeText.ToString(),
                    Lines = null,
                    Words = null
                };
            }
            else
            { 
                return new PdfMetadata
                {
                    Text = completeText.ToString(),
                    Lines = null,
                    Words = null
                };
            }
        }

        public async Task<PdfMetadata> Extract(Stream stream, bool detailed = false)
        {

            string path = Path.Combine(Directory.GetCurrentDirectory(), "file");

            using (var Filestream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(Filestream);
            }


            PdfDocument pdfDoc = new PdfDocument(new PdfReader(path));

            if (detailed)
            {
                return new PdfMetadata
                {
                    Text = ""
                };
            }
            else
            {
                int countPages = pdfDoc.GetNumberOfPages();

                StringBuilder completeText = new StringBuilder();
                for (int i = 1; i < countPages + 1; i++)
                {
                    PdfPage p = pdfDoc.GetPage(i);
                    completeText.Append(PdfTextExtractor.GetTextFromPage(p));
                }
                pdfDoc.Close();

                return new PdfMetadata
                {
                    Text = completeText.ToString(),
                    Lines = null,
                    Words = null
                };
            };
        }
    }
}

