﻿using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PDFIndexer.TextStructures
{
    public class ShowTitleSegment : ILogStructurePdf<TextSegment>
    {
        int _id = 0;

        public void EndLogPdf(IPipelineDebug pipeline)
        {
        }

        public void LogPdf(IPipelineDebug pipeline, TextSegment data)
        {
            var titles = data.OriginalTitle;

            if (titles.Length == 0)
                return;

            for(int i=0; i<titles.Length-1; i++)
            {
                var lines = titles[i].Lines;
                pipeline.ShowLine(lines, Color.Orange);
                pipeline.ShowText($"H{i+1}", lines[0], Color.Orange);
            }

            var lastTitleLines = titles[titles.Length - 1].Lines;

            pipeline.ShowLine(lastTitleLines, Color.Green);
            pipeline.ShowText(_id.ToString(), lastTitleLines[0], Color.Green);

            _id++;
        }

        public void StartLogPdf(IPipelineDebug pipeline)
        {
        }
    }
}
