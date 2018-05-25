using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer
{
    interface IConvertBlock
    {
        IEnumerable<TextLine> ProcessPage(int pageNumber, BlockPage page);
    }
}
