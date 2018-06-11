using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFIndexer.Search
{
    class DataForTest
    {
        public static IndexMetadata Get(Guid id)
        {
            return GetAll().SingleOrDefault(x => x.Id.Equals(id));
        }
        public static List<IndexMetadata> GetAll()
        {
            string path = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO NÚMERO 05 - BANANA PASSA.pdf";

            string path2 = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO NÚMERO SETE - CAJU.pdf";

            string path3 = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO OITO - CUMBARÚ.pdf";


            TextExtractor te = new TextExtractor();
            return new List<IndexMetadata>
            {
                te.GetIndexMetadata(path),
                te.GetIndexMetadata(path2),
                te.GetIndexMetadata(path3)
            };
        }
    }
}

