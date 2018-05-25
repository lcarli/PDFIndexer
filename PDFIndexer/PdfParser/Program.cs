using System;
using System.IO;
using System.Diagnostics;

namespace PDFIndexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("PDF Text Reader");
            var watch = Stopwatch.StartNew();

            Program3.ProcessStage("DO1_2005_01_05");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
