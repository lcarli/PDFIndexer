﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.CommomModels
{
    public class PdfMetadata
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int page { get; set; }

    }
}
