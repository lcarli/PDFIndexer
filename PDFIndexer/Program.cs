using PDFIndexer.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            LuceneSearch s = new LuceneSearch();

            var result = s.Search("banana");

            Console.ReadKey();
        }
    }
}
