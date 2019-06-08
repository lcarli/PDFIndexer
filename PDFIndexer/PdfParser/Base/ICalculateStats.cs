using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface ICalculateStats<T>
    {
        object Calculate(IEnumerable<T> stats);
    }
}
