using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IProcessBlockData : IProcessBlock
    {
        void UpdateInstance(object cache);
        BlockPage LastResult { get; }
    }
}
