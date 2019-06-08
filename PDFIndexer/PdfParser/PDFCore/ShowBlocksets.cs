using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.PDFCore
{
    public class ShowBlocksets : IProcessBlock
    {
        public BlockPage Process(BlockPage page2)
        {
            var page = page2 as BlockPage2;

            if (page == null)
                PdfReaderException.AlwaysThrow("ShowBlocksets must execute AFTER OrganizePageLayout");

            var blocksets = new BlockPage();

            foreach(var seg in page.Segments)
            {
                blocksets.AddRange(seg.Columns);
            }

            return blocksets;
        }
    }
}
