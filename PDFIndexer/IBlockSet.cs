using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer
{
    public interface IBlockSet
    {
    }
    public interface IBlockSet<T> : IBlock, IEnumerable<T>
    {
    }

    public interface IBlockArea : IBlockSet<IBlock>
    {
    }
}
