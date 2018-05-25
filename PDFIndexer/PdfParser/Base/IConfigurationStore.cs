using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IConfigurationStore
    {
        string Get(string filename);
    }
}
