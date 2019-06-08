using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Parser
{
    public class CreateTextLineIndex : IAggregateStructure<TextLine, TextLine>
    {
        // this class does nothing
        // however, it indirectly creates an index for TextLine
        public bool Aggregate(TextLine line)
        {
            return false;
        }

        public TextLine Create(List<TextLine> lines)
        {
            return lines[0];
        }

        public void Init(TextLine line)
        {
        }
    }
}
