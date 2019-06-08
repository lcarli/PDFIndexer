﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFIndexer.Base
{
    public class TextSegment
    {
        public TextStructure[] OriginalTitle { get; set; }
        public TextStructure[] Title { get; set; }
        public TextStructure[] Body { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("==========================================================================");
            sb.AppendLine();
            sb.AppendLine(String.Join("\r\n", Title.Select(t => t.Text)));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine(String.Join("\r\n", Body.Select(t => t.Text)));
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
