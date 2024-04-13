using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using System.IO.Compression;

namespace StudentContributions.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MagazineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHost;

        public MagazineController(IUnitOfWork unitOfWork, IWebHostEnvironment webhost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webhost;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DownloadZipMagazine(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var contributions = _unitOfWork.ContributionRepository.GetAll(c => c.MagazineID == id && c.Contribution_Status.Contains("Approved"));
            if (contributions == null)
            {
                return NotFound();
            }

            MemoryStream memoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var con in contributions)
                {
                    string foldername = "Article_" + con.ID.ToString() + "_" + con.Title;
                    zipArchive.CreateEntry(foldername + "/");
                    string[] paths = Directory.GetFiles(Path.Combine(_webHost.WebRootPath, "Contributions", con.ID.ToString()));
                    foreach (var file in paths)
                    {
                        zipArchive.CreateEntryFromFile(file, foldername + "/"+Path.GetFileName(file));
                    }
                }
                
            }

            memoryStream.Position = 0;

            var mag = _unitOfWork.MagazineRepository.Get(m => m.ID == id);
            string filename = "Magazine_" + id.ToString() + "_" + mag.MagazineName + ".zip";
            return File(memoryStream, "application/zip", filename);
        }
    }
}
