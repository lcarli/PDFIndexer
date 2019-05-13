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
            string path = @"{path of document}";
            ProcessPDF.AddPDFs(new List<string>() { path });

            var result = ProcessPDF.GetVisualResults("{your search word}");

            string json = JsonConvert.SerializeObject(result);

            string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);

            Console.WriteLine(jsonFormatted);

            Console.ReadKey();
        }
    }
}
