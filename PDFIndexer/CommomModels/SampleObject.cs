using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.CommomModels
{
    class SampleObject
    {
        public IndexMetadata Metadata { get; set; }
        public HighlightObject HighlightObject { get; set; }
        public string ImageUri { get; set; }
        public string Sample { get; set; }
    }
}
