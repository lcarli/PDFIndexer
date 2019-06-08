using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer.Configuration
{
    public class ConfigurationFile : IConfigurationStore
    {
        public string Get(string filename)
        {
            string content = null;
            try
            {
                content = File.ReadAllText(filename);
            }
            catch { }

            return content;
        }
    }
}
