using PDFIndexer.CommomModels;
using PDFIndexer.Search;
using PDFIndexer.Utils;
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
            //TESTE
            LuceneSearch.AddUpdateLuceneIndex<IndexMetadata>(DataForTest.GetAll());

            var result = LuceneSearch.SearchDefault<IndexMetadata>("banana", "Text");

            var resultAll = LuceneSearch.GetAllIndexRecords<IndexMetadata>();

            var bla = ProcessResult.ProcessResults(resultAll, "banana");

            Console.ReadKey();
        }
    }
}
