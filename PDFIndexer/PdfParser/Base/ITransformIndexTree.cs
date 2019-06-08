using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface ITransformIndexTree
    {
        int FindPageStart<T>(T instance);
    }
}
