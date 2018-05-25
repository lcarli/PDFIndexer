using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Execution
{
    interface IPipelineContext
    {
    }
    interface IPipelinePdfContext : IPipelineContext
    {
        PipelineInputPdf.PipelineInputPdfPage CurrentPage { get; }

        IProcessBlockData FromCache<T>(int pageNumber);

        void StoreCache<T>(int pageNumber, IProcessBlockData result);

    }
}
