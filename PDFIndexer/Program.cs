using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PDFIndexer.Utils;
using System;
using System.Collections.Generic;


namespace PDFIndexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\lucas\Desktop\IBAgro\BLOG - ARTIGO NÚMERO 05 - BANANA PASSA.pdf";
            ProcessPDF.AddPDFs(new List<string>() { path });

            var result = ProcessPDF.GetVisualResults("banana");

            string json = JsonConvert.SerializeObject(result);

            string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);

            Console.WriteLine(jsonFormatted);

            Console.ReadKey();
        }
    }
}
