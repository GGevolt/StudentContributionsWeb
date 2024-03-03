using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models;
using StudentContributions.Models.ViewModels;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;

namespace StudentContributions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            string[] paths = Directory.GetFiles(Path.Combine(this._webHostEnvironment.WebRootPath, "FilesTest/"));

            List<HomeTestVM> files = new List<HomeTestVM>();
            foreach (string file in paths)
            {
                files.Add(new HomeTestVM { FileName = Path.GetFileName(file) });
            }

            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Path.Combine(this._webHostEnvironment.WebRootPath, "FilesTest/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        public FileResult DownloadZip(/*string fileName*/)
        {
            string[] paths = Directory.GetFiles(Path.Combine(this._webHostEnvironment.WebRootPath, "FilesTest/"));

            MemoryStream memoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in paths)
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream, "application/zip", "bokita.zip");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
