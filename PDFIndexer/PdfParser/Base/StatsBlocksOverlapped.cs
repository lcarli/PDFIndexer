using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public class StatsBlocksOverlapped
    {
        public static StatsBlocksOverlapped Empty = new StatsBlocksOverlapped();

        public IBlock[] Blocks;
        public int[] BlockIds;
    }
}
