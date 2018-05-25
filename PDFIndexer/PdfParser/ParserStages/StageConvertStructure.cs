using PDFIndexer.Base;
using PDFIndexer.Configuration;
using PDFIndexer.Execution;
using PDFIndexer.Parser;
using PDFIndexer.PDFCore;
using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.ParserStages
{
    class StageConvertStructure
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertStructure(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextStructure>();

            var resultPipeline = pipelineText
                                    .ConvertText<CreateTextSegments, TextSegment>()
                                    .ConvertText<FilterTextSegments, TextSegment>()
                                    .ConvertText<AfterFilterTextSegments, TextSegment>();

            _context.SetPipelineText<TextSegment>(resultPipeline);
        }

        public void ProcessWithConfiguration(string filename)
        {
            _context.Config<ParserTreeConfig>(filename);

            var pipelineText = _context.GetPipelineText<TextStructure>();

            var resultPipeline = pipelineText
                                    .ConvertText<CreateTextSegmentsWithConfigData, TextSegment>();

            _context.SetPipelineText<TextSegment>(resultPipeline);
        }
    }
}
