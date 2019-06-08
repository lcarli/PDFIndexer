using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IProcessBlock
    {
        BlockPage Process(BlockPage page);
    }
}
