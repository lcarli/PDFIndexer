﻿using PDFIndexer.Execution;
using PDFIndexer.PDFCore;
using PDFIndexer.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PDFIndexer.ParserStages
{
    public class StagePageMargins
    {
        private readonly StageContext _context;

        public StagePageMargins(StageContext context)
        {
            this._context = context;
        }

        public void Process()
        {
            Pipeline pipeline = _context.GetPipeline();
            
            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .StageProcess(FindMargins);

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage1-margins.pdf")
                    .StageProcess(ShowColors);
        }

        void FindMargins(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<IdentifyTablesData>()
                .FromCache<ProcessImageData>()
                .FromCache<ProcessPdfTextData>()
                  .ParseBlock<FindDouHeaderFooter>()
                        .StoreCache<HeaderFooterData>();
        }

        void ShowColors(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<HeaderFooterData>()
                .FromCache<ProcessPdfTextData>()
                  .Validate<ShowTextHeaderFooter>().ShowErrors(p => p.Show(Color.PaleVioletRed))
                  .ParseBlock<ShowTextHeaderFooter>()
                    .Show(Color.Yellow);
        }
    }
}
