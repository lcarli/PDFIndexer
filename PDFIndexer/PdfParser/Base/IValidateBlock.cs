using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IValidateBlock
    {
        BlockPage Validate(BlockPage page);
    }
}
