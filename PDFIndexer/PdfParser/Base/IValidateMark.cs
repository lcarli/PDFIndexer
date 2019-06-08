using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IValidateMark
    {
        string Validate(BlockSet<MarkLine> marks);
    }
}
