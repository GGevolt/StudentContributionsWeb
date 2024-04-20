using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;
using System.IO.Compression;

namespace StudentContributions.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ArticlesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHost;


        public ArticlesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id);
            if (contribution == null)
            {
                return NotFound();
            }
            ConDetails details = new ConDetails();
            details.Filenames = new List<string>();
            details.Contribution = contribution;
            string path = Path.Combine(this._webHost.WebRootPath, "Contributions", contribution.ID.ToString());
            if (Directory.Exists(path))
            {
                string[] paths = Directory.GetFiles(Path.Combine(_webHost.WebRootPath, "Contributions", contribution.ID.ToString()));
                foreach (string file in paths)
                {
                    details.Filenames.Add(Path.GetFileName(file));
                }
            }
            return View(details);
        }

        public FileResult DownloadFile(string fileName, int id)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString() + "/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        public FileResult DownloadZip(int id)
        {
            string[] paths = Directory.GetFiles(Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString()));

            MemoryStream memoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in paths)
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            memoryStream.Position = 0;

            var con = _unitOfWork.ContributionRepository.Get(c => c.ID == id);
            string filename = "Article_" + id.ToString() + "_" + con.Title + ".zip";
            return File(memoryStream, "application/zip", filename);
        }
    }
}
