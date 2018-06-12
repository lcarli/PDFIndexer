using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.CommomModels
{
    class HighlightObject
    {
        public IndexMetadata Metadata { get; set; }
        public List<BoundingBox> HighlightedWords { get; set; }
        public string Keyword { get; set; }
        public int PageNumber { get; set; }
    }

    class BoundingBox
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
