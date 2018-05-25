using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IValidateMark
    {
        string Validate(BlockSet<MarkLine> marks);
    }
}
