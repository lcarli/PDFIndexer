using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.CommomModels
{
    class SampleObject
    {
        public IndexMetadata Metadata { get; set; }
        public byte[] Image { get; set; }
        public byte[] Sample { get; set; }
    }
}
