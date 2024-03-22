using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;
using System.IO;
using System.IO.Compression;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
    //[Authorize(Roles = "Student,Coordinator")]
    public class ContributionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHost;

        public ContributionController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            var contributions = _unitOfWork.ContributionRepository.GetAll();
            return View(contributions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contribution contribution, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ContributionRepository.Add(contribution);
                _unitOfWork.Save();

                string uploadPath = Path.Combine(this._webHost.WebRootPath, "Contributions", contribution.ID.ToString());
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contribution);
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

        [HttpPost]
        public IActionResult Details(List<IFormFile>? files, int id)
        {
            string uploadPath = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString());
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        public FileResult DownloadFile(string fileName, int id)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString()+"/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        public IActionResult DeleteFile(string fileName, int id)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString() + "/") + fileName;

            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }

            return RedirectToAction("Details", new { id =  id});
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
            string filename = "Article_"+id.ToString()+"_"+ con.Title+".zip";
            return File(memoryStream, "application/zip", filename);
        }

        public IActionResult Edit(int? id)
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
            return View(contribution);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contribution contribution)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ContributionRepository.Update(contribution);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(contribution);
        }
        [Authorize(Roles = "Student")]
        public IActionResult Delete(int? id)
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
            return View(contribution);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public IActionResult DeleteConfirmed(int id)
        {
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id);
            if (contribution != null)
            {
                _unitOfWork.ContributionRepository.Remove(contribution);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
