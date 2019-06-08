using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IConvertBlock
    {
        IEnumerable<TextLine> ProcessPage(int pageNumber, BlockPage page);
    }
}
