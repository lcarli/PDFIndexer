using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Execution
{
    interface IPipelineResults<T>
    {
        T GetResults();
    }
}
