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
using PDFIndexer.Execution;
using PDFIndexer.PdfParser.TextStructures;
using PDFIndexer.PDFText;
using PDFIndexer.PdfParser.PDFCore;
using PDFIndexer.Base;

namespace PDFIndexer
{
    class TextExtractor
    { 
        public Pipeline pipeline;
        public TextExtractor()
        {
            pipeline = new Pipeline();
            PdfReaderException.ContinueOnException();
        }
        public List<PdfMetadata> Extract(string path, bool detailed = false)
        {
            var result = pipeline.Input(path)
                                .AllPages<CreateWordBlock>(page =>
                                    page.ParsePdf<ProcessPdfText>()
                                        .ParseBlock<GroupWords>()
                                    );
            var conteudo = result.ToList();
            return ConvertTextLineToMetadata(conteudo);
        }

        public async Task<List<PdfMetadata>> Extract(Stream stream, bool detailed = false)
        {

            string path = Path.Combine(Directory.GetCurrentDirectory(), "file");

            using (var Filestream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(Filestream);
            }

            var result = pipeline.Input(path)
                                .AllPages<CreateWordBlock>(page =>
                                    page.ParsePdf<ProcessPdfText>()
                                        .ParseBlock<GroupWords>()
                                    );
            var conteudo = result.ToList();
            return ConvertTextLineToMetadata(conteudo);
        }

        private List<PdfMetadata> ConvertTextLineToMetadata(IList<TextLine> list)
        {
            List<PdfMetadata> metadataList = new List<PdfMetadata>();
            foreach (var item in list)
            {
                var metadataItem = new PdfMetadata
                {
                    Text = item.GetText(),
                    X = item.GetX(),
                    H = item.GetH(),
                    Width = item.GetWidth(),
                    Height = item.GetHeight()
                };
                metadataList.Add(metadataItem);
            }
            return metadataList;
        }
    }
}

