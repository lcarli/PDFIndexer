﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public class TextStructureAgg
    {
        public TextStructure TextStruct;
        public bool SameFont;
        public bool SameSpacing;
        public bool AlignedTabStop;
        public bool HasContinuation;
        public float VerticalSpacing;
    }
}
