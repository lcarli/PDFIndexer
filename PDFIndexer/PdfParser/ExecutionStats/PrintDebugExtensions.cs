using PDFIndexer.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.ExecutionStats
{
    public static class PrintDebugExtensions
    {
        public static PipelineText<T> DebugCount<T>(this PipelineText<T> pipelineText)
        {
            return pipelineText.Log<PrintDebugCount<T>>(Console.Out);
        }

        public static PipelineText<T> DebugPrint<T>(this PipelineText<T> pipelineText)
        {
            return pipelineText.Log<PrintDebugPrint<T>>(Console.Out);
        }
    }
}
