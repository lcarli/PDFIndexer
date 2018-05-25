﻿using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDFIndexer.TextStructures
{
    class AnalyzeLinesCenterRight : ILogStructure<TextLine>
    {
        public void StartLog(TextWriter input)
        {
        }

        public void Log(TextWriter input, TextLine line)
        {
           

            float? afterSpace = line.AfterSpace;
            float? beforeSpace = line.BeforeSpace;


            float? lineCenter = (float)line.CenteredAt;

            if (lineCenter < 29 && lineCenter > 28)
            {
                input.WriteLine("-----------------------------------");
                input.WriteLine($"Margins: (LEFT: {line.MarginLeft}, RIGHT: {line.MarginRight}, CENTER: {line.CenteredAt})");

                input.Write($"TEXT: {line.Text}");
                input.WriteLine($" ({line.FontName}, {line.FontSize.ToString("0.00")}, {line.FontStyle})");
                input.WriteLine($" (AfterSpace: {afterSpace})");
                input.WriteLine($" (BeforeSpace: {afterSpace})");
                input.WriteLine();

                input.WriteLine("");


            }
    
        }

        public void EndLog(TextWriter input)
        {
        }
    }
}
