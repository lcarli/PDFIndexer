﻿using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDFIndexer.TextStructures
{
    public class AnalyzeStructures : ILogStructure<TextStructure>
    {
        public void StartLog(TextWriter input)
        {
        }
        
        public void Log(TextWriter input, TextStructure structure)
        {
            input.WriteLine("-----------------------------------");

            float? afterSpace = structure.AfterSpace;

            input.WriteLine($"Aligment: {structure.TextAlignment}");

            input.Write(structure.Text);
            input.WriteLine($" ({structure.FontName}, {structure.FontSize.ToString("0.00")}, {structure.FontStyle} - {structure.HasBackColor})");
            input.WriteLine($" ({afterSpace})");
            input.WriteLine();

            input.WriteLine("");
        }

        public void EndLog(TextWriter input)
        {
        }
    }
}
