using PDFIndexer.Base;
using PDFIndexer.Parser;
using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.ParserStages
{
    class StageConvertContent
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertContent(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextSegment>();

            var resultPipeline = pipelineText
                            .ConvertText<CreateContent, TextSegment>();

            _context.SetPipelineText<TextSegment>(resultPipeline);
        }
    }
}
