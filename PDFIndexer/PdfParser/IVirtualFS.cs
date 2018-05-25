using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer
{
    public interface IVirtualFS
    {
        Stream OpenReader(string filename);
        Stream OpenWriter(string filename);
    }
}
