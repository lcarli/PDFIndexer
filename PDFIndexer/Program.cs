using PDFIndexer.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var result = LuceneSearch.SearchDefault("banana");

            Console.ReadKey();
        }
    }
}
