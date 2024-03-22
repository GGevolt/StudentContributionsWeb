using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models.ViewModels;
using StudentContributions.Models.Models;
using System.Diagnostics;
using System.IO.Compression;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
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
            string[] paths = Directory.GetFiles(Path.Combine(_webHostEnvironment.WebRootPath, "FilesTest/"));

            List<HomeTestVM> files = new List<HomeTestVM>();
            foreach (string file in paths)
            {
                files.Add(new HomeTestVM { FileName = Path.GetFileName(file) });
            }

            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "FilesTest/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        public FileResult DownloadZip(/*string fileName*/)
        {
            string[] paths = Directory.GetFiles(Path.Combine(_webHostEnvironment.WebRootPath, "FilesTest/"));

            MemoryStream memoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in paths)
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream, "application/zip", "btr.zip");
        }

        public IActionResult DeleteFile(string fileName)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "FilesTest/") + fileName;

            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Index(List<IFormFile>? files)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "FilesTest");

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return RedirectToAction("Index");
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
