using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IBlock
    {
        string GetText();
        float GetX();
        float GetH();
        float GetWidth();
        float GetHeight();
        float GetWordSpacing();
    }
}
