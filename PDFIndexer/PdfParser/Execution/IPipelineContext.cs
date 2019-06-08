using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Execution
{
    public interface IPipelineContext
    {
    }
    public interface IPipelinePdfContext : IPipelineContext
    {
        PipelineInputPdf.PipelineInputPdfPage CurrentPage { get; }

        IProcessBlockData FromCache<T>(int pageNumber);

        void StoreCache<T>(int pageNumber, IProcessBlockData result);

    }
}
