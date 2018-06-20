using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer
{
    public static class Config
    {
        public static string ImageStorageConn { get; set; }
        public static string PdfStorageConn { get; set; }

        public static string TemporatyPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "_temp");

    }
}
