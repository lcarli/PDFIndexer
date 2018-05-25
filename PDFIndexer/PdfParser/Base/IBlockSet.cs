using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IBlockSet 
    {
    }

    interface IBlockSet<T> : IBlock, IEnumerable<T>
    {
    }

    interface IBlockArea : IBlockSet<IBlock>
    {
    }
}
