using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IProcessStructure<T>
    {
        T Process(T structure);
    }

    public interface IProcessStructure2<T>
    {
        IEnumerable<T> Process(IEnumerable<T> input);
    }
}
