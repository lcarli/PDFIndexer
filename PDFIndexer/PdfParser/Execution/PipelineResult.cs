using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Execution
{
    public interface IPipelineResults<T>
    {
        T GetResults();
    }
}
