using PDFIndexer.CommomModels;
using PDFIndexer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFIndexer.Search
{
    class DataForTest
    {
        public static List<IndexMetadata> GetAll()
        {
            string path = @"C:\Users\lucas\Desktop\IBAgro";

            TextExtractor te = new TextExtractor();
            return te.GeAlltIndexMetadata(path);
        }
    }
}

