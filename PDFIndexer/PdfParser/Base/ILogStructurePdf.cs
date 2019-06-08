using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface ILogStructurePdf<T>
    {
        void StartLogPdf(IPipelineDebug pipeline);
        void LogPdf(IPipelineDebug pipeline, T data);
        void EndLogPdf(IPipelineDebug pipeline);
    }
}
