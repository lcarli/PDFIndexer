using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.PdfParser.TextStructures
{
    class CreateWordBlock : IConvertBlock
    {
        public IEnumerable<TextLine> ProcessPage(int pageNumber, BlockPage page)
        {
            foreach (var bset in page.AllBlocks)
            {
                var bline = bset as BlockLine;

                var newLine = new TextLine
                {
                    Text = bline.Text,
                    FontName = bline.FontName,
                    FontSize = bline.FontSize,
                    FontStyle = bline.FontStyle,
                    Block = bline
                };

                yield return newLine;
            }
        }
    }
}
