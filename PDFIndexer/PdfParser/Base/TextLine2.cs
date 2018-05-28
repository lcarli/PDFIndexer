using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    class TextLine2 : TextLine
    {
        public bool AlignedCenter;
        public bool HasContinuation;

        public TextLine2(TextLine old)
        {
            this.Text = old.Text;
            this.PageInfo = old.PageInfo;
            this.MarginRight = old.MarginRight;
            this.MarginLeft = old.MarginLeft;
            this.HasLargeSpace = old.HasLargeSpace;
            this.HasBackColor = old.HasBackColor;
            this.FontStyle = old.FontStyle;
            this.FontSize = old.FontSize;
            this.FontName = FontName;
            this.CenteredAt = old.CenteredAt;
            this.Block = old.Block;
            this.BeforeSpace = old.BeforeSpace;
            this.AfterSpace = old.AfterSpace;
        }

        public TextLine2() { }
        
    }
}
