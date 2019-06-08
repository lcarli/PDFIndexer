﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IPipelineDebug
    {
        void ShowLine(TextLine line, System.Drawing.Color color);
        void ShowLine(IEnumerable<TextLine> lines, System.Drawing.Color color);
        void ShowText(string text, TextLine line, System.Drawing.Color color);
    }
}
