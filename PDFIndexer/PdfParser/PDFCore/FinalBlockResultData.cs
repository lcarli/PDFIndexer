using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.PDFCore
{
    public class FinalBlockResultData : IProcessBlockData
    {
        public BlockPage LastResult { get; private set; }

        public BlockPage Process(BlockPage page)
        {
            LastResult = page;
            return page;
        }

        public void UpdateInstance(object cache)
        {
        }
    }
}
