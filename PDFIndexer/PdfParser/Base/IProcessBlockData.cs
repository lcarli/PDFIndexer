using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IProcessBlockData : IProcessBlock
    {
        void UpdateInstance(object cache);
        BlockPage LastResult { get; }
    }
}
