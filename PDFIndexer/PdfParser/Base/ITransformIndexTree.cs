using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface ITransformIndexTree
    {
        int FindPageStart<T>(T instance);
    }
}
