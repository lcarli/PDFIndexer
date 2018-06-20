using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace PDFIndexer.Services
{
    class ImageProcessing
    {
        static readonly JpegDecoder JPEG = new JpegDecoder();

        public static Stream Crop(Stream stream, float tx, float ty, float tw, float th)
        {
            Stream output = new MemoryStream();

            using (Image<Rgba32> image = Image.Load(stream, JPEG))
            {
                int x1 = (int)(image.Width * tx);
                int y1 = (int)(image.Height * ty);
                int dx = (int)(image.Width * tw);
                int dy = (int)(image.Height * th);

                image.Mutate(x => x
                    .Crop(new Rectangle(x1, y1, dx, dy))
                    //.Resize(image.Width / 2, image.Height / 2)
                    );

                image.Save(output, new JpegEncoder());
            }

            output.Seek(0, SeekOrigin.Begin);

            return output;
        }

        public static List<string> UploadImages(Stream[] pdfPageImageList, string filename)
        {
            //Upload Pages in Patch
            var container = GetContainer(Config.ImageStorageConn, "imagepdf");

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            container.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            }).GetAwaiter().GetResult();    


            var tasks = new List<Task>();
            var rand = new Random(DateTime.Now.Second);
            int value = rand.Next();
            var list = new List<string>();
            
            for (int page = 0; page < pdfPageImageList.Length; page++)
            {
                var blobName = $"{filename}/page_{page + 1}.jpg";
                var blob = container.GetBlockBlobReference(blobName);
                blob.Properties.ContentType = "image/jpg";
                tasks.Add(blob.UploadFromStreamAsync(pdfPageImageList[page]));
                list.Add(blob.Uri.ToString());
            }

            Task.WaitAll(tasks.ToArray());

            return list;
        }

        public static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            string conn = connectionString;

            var storageAccount = CloudStorageAccount.Parse(conn);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            return container;
        }
    }
}
