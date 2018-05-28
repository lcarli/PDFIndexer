﻿using System;
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
using PDFIndexer.PDFCore;
using PDFIndexer.Parser;
using PDFIndexer.TextStructures;

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
        public List<PdfMetadata> ExtractWordsMetadata(string path, bool detailed = false)
        {
            var result = pipeline.Input(path)
                                .AllPages<CreateWordBlock>(page =>
                                    page.ParsePdf<ProcessPdfText>()
                                        .ParseBlock<GroupWords>()
                                    );
            var conteudo = result.ToList();
            return ConvertTextLineToMetadata(ConvertToTextLine2(conteudo));
        }

        public async Task<List<PdfMetadata>> ExtractWordsMetadata(Stream stream, bool detailed = false)
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
            return ConvertTextLineToMetadata(ConvertToTextLine2(conteudo));
        }

        public List<PdfMetadata> ExtractLinesMetadata(string path, bool detailed = false)
        {
            var result = pipeline.Input(path)
                                .AllPages<CreateWordBlock>(page =>
                                    page.ParsePdf<ProcessPdfText>()
                                        .ParseBlock<GroupLines>()
                                    );

            var conteudo = result.ConvertText<CreateTextLineIndex, TextLine>()
                                .ConvertText<PreCreateStructures, TextLine2>()
                                .ToList();



            return ConvertTextLineToMetadata(conteudo);
        }

        public async Task<List<PdfMetadata>> ExtractLinesMetadata(Stream stream, bool detailed = false)
        {

            string path = Path.Combine(Directory.GetCurrentDirectory(), "file");

            using (var Filestream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(Filestream);
            }

            var result = pipeline.Input(path)
                                .AllPages<CreateWordBlock>(page =>
                                    page.ParsePdf<ProcessPdfText>()
                                        .ParseBlock<GroupLines>()
                                    );
            var conteudo = result.ConvertText<CreateTextLineIndex, TextLine>()
                                 .ConvertText<PreCreateStructures, TextLine2>()
                                 .ToList();

            return ConvertTextLineToMetadata(conteudo);
        }

        private List<PdfMetadata> ConvertTextLineToMetadata(IList<TextLine2> list)
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

        private List<TextLine2> ConvertToTextLine2(IList<TextLine> oldList)
        {
            List<TextLine2> newList = new List<TextLine2>();
            foreach (var item in oldList)
            {
                newList.Add(new TextLine2(item));
            }
            return newList;
        }
    }
}

