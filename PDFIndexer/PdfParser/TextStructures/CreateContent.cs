using PDFIndexer.Base;
using PDFIndexer.Execution;
using PDFIndexer.Parser;
using PDFIndexer.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.TextStructures
{
    class CreateContent : IAggregateStructure<TextSegment, TextSegment>
    {
        public CreateContent(BasicFirstPageStats basicFirstPageStats, PipelinePageStats<int> teste, PipelineDocumentStats docstats)
        {
        }

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> input)
        {
            return input[0];
        }

        public void Init(TextSegment line)
        {
        }
    }
}
