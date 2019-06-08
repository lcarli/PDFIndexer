using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PDFIndexer.CommomModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PDFIndexer.Utils
{
    public static class ProcessPDF
    {
        public static async Task AddPDFs(List<string> uris)
        {
            //Instance of TextExtractor
            TextExtractor te = new TextExtractor();

            //List of IndexMetadata for LUCENE
            List<IndexMetadata> metadatas = new List<IndexMetadata>();

            //Process each pdf
            foreach (var item in uris)
            {
                //Upload PDF to Blob
                var pdfLink = await UploadPDFs(item);

                //Process IndexMetadata for LUCENE
                metadatas.Add(ProcessMetadata(item, te));

                //Convert all pdf pages to images
                var imageUris = Convert2Image(item);
            }

            //Index to LUCENE
            LuceneSearch.AddUpdateLuceneIndex<IndexMetadata>(metadatas);

            await Task.CompletedTask;

        }

        public static async Task<List<SampleObject>> GetVisualResults(string keyword)
        {
            var result = LuceneSearch.SearchDefault<IndexMetadata>(keyword, "Text");

            return await ProcessResult.ProcessResults(result, keyword);
        }

        private static List<string> Convert2Image(string path)
        {
            return ProcessResult.ConvertPdf2Image(path);
        }

        private static IndexMetadata ProcessMetadata(string path, TextExtractor te)
        {
            return te.GetIndexMetadata(path);
        }

        private static async Task<string> UploadPDFs(string path)
        {
            var container = GetContainer(Config.PdfStorageConn, "rawpdf");

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            container.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            }).GetAwaiter().GetResult();

            var blob = container.GetBlockBlobReference(Path.GetFileNameWithoutExtension(path));
            blob.Properties.ContentType = "application/pdf";
            await blob.UploadFromFileAsync(path);
            return blob.Uri.ToString();
        }

        private static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            string conn = connectionString;

            var storageAccount = CloudStorageAccount.Parse(conn);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            return container;
        }
    }
}
