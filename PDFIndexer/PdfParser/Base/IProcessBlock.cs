﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IProcessBlock
    {
        BlockPage Process(BlockPage page);
    }
}