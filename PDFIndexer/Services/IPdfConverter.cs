using System.IO;


namespace PDFIndexer.Services
{
    interface IPdfConverter
    {
        void GenerateImage(Stream pdfInput, ref Stream[] imageListOutput);
    }
}
