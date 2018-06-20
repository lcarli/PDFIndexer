using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeautifulPdfSearch.Models;
using Microsoft.Extensions.Configuration;
using BeautifulPdfSearch.Extensions.Alerts;
using Microsoft.AspNetCore.Http;
using System.IO;
using PDFIndexer.CommomModels;
using PDFIndexer.Utils;

namespace BeautifulPdfSearch.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult Index(List<SampleObject> result = null)
        {
            Config.ImageStorageConn = Configuration.GetSection("Storage")["imageurl"];
            Config.PdfStorageConn = Configuration.GetSection("Storage")["pdfurl"];

            return View(result);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Search(string text)
        {
            List<SampleObject> result = await ProcessPDF.GetVisualResults(text);

            //Do something with the result

            return RedirectToAction("Index", result);
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Add(List<IFormFile> files)
        {
            List<string> list = new List<string>();

            long size = files.Sum(f => f.Length);

            // full path to file in temp 

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempPath() + formFile.FileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    list.Add(filePath);
                }
            }

            await ProcessPDF.AddPDFs(list);

            //return View("Index").WithSuccess("Sucess: ", "All PDFs was saved");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
