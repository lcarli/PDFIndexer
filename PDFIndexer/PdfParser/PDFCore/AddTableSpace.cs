﻿using PDFIndexer.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.PDFCore
{
    class AddTableSpace : IProcessBlock
    {
        private List<IBlock> _tables;

        public AddTableSpace(PDFCore.IdentifyTables parserTable)
        {
            var page = parserTable.PageTables;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("AddTableSpace requires IdentifyTables");
            }

            this._tables = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._tables == null)
            {
                PdfReaderException.AlwaysThrow("AddTableSpace requires IdentifyTables");
            }

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                result.Add(block);
            }
            foreach (var block in _tables)
            {
                result.Add(block);
            }

            return result;
        }
    }
}
