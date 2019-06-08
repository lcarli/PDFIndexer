using PDFIndexer.Base;
using PDFIndexer.Execution;
using PDFIndexer.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.ExecutionStats
{
    public static class PrintAnalyticsExtensions
    {
        public static PipelineText<TextLine> PrintAnalytics(this PipelineText<TextLine> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowTextLine>(filename);
        }

        public static PipelineText<TextStructure> PrintAnalytics(this PipelineText<TextStructure> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowTextStructure>(filename);
        }

        public static PipelineText<Content> PrintAnalytics(this PipelineText<Content> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowConteudo>(filename);
        }

    }
}
