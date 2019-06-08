﻿using PDFIndexer.Execution;
using PDFIndexer.PDFCore;
using PDFIndexer.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PDFIndexer.ParserStages
{
    public class StagePdfInput
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StagePdfInput(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            string basename = _context.Basename;
            Pipeline pipeline = _context.GetPipeline();

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage0-input.pdf")
                    .StageProcess(InitialCache);
        }

        void InitialCache(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                    .Show(Color.Red)
                    .ParseBlock<IdentifyTables>()
                    .ParseBlock<SetIdentifyTablesCompatibility>()
                    .StoreCache<IdentifyTablesData>()
                    .Show(Color.Orange)
                .ParsePdf<PreProcessImages>()
                    .StoreCache<ProcessImageData>()
                    .Show(Color.Yellow)
                .ParsePdf<ProcessPdfText>()
                    .StoreCache<ProcessPdfTextData>()
                    .Show(Color.Black);
        }
    }
}
