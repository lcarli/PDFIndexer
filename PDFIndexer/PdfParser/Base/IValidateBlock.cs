using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IValidateBlock
    {
        BlockPage Validate(BlockPage page);
    }
}
