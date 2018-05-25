using System;
using System.Threading.Tasks;
using PDFIndexer;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO NÚMERO 05 - BANANA PASSA.pdf";
            Get(path, true);
            Console.ReadKey();
        }

        static void Get(string a, bool b)
        {
            TextExtractor e = new TextExtractor();
            var aaa =  e.Extract(a, b);
        }
    }
}
