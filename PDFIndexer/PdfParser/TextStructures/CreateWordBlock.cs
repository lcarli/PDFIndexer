using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.PdfParser.TextStructures
{
    public class CreateWordBlock : IConvertBlock
    {
        public IEnumerable<TextLine> ProcessPage(int pageNumber, BlockPage page)
        {
            foreach (var bset in page.AllBlocks)
            {
                int blockId = 0;
                var bline = bset as BlockLine;

                var pageInfo = new TextPageInfo()
                {
                    PageNumber = pageNumber,
                    BlockId = blockId
                };

                var newLine = new TextLine
                {
                    Text = bline.Text,
                    FontName = bline.FontName,
                    FontSize = bline.FontSize,
                    FontStyle = bline.FontStyle,
                    Block = bline,
                    PageInfo = pageInfo
                };

                blockId++;
                yield return newLine;
            }
        }
    }
}
