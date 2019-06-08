using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    public interface IConfigurationStore
    {
        string Get(string filename);
    }
}
