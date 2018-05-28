using System;
using System.Threading.Tasks;
using PDFIndexer;
using PDFIndexer.Base;
using PDFIndexer.Parser;
using PDFIndexer.ExecutionStats;
using PDFIndexer.TextStructures;
using PDFIndexer.Execution;
using PDFIndexer.PDFText;
using PDFIndexer.PDFCore;
using System.Drawing;
using PDFIndexer.PdfParser.TextStructures;
using PDFIndexer.PdfParser.PDFCore;

namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO NÚMERO 05 - BANANA PASSA.pdf";


            TextExtractor te = new TextExtractor();
            var list = te.ExtractLinesMetadata(path, true);

            Console.ReadKey();
        }
    }
}
