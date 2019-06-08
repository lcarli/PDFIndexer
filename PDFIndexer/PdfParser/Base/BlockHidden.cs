using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public class BlockHidden : Block
    {
        public string GetHiddenText()
        {
            return TitleWithHiddenIdMateria.GetHiddenText(Text);
        }
    }
}
