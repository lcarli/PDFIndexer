﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    class BlockHidden : Block
    {
        public string GetHiddenText()
        {
            return TitleWithHiddenIdMateria.GetHiddenText(Text);
        }
    }
}
