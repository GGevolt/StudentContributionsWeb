using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;
using StudentContributions.Utility.Interfaces;
using System.IO.Compression;
using System.Text.Encodings.Web;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student,Coordinator")]
    public class ContributionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHost;

        public ContributionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailService emailService, IWebHostEnvironment webhost)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _webHost = webhost;
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
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user != null)
            {
                contribution.UserID = user.Id;
                var facultyID = _unitOfWork.MagazineRepository.Get(m => m.ID == contribution.MagazineID).FacultyID;
                var usersInFaculty = _unitOfWork.ApplicationUserRepository.GetAll(u => u.FacultyID == facultyID);
                var usersAsCoordinator = _userManager.GetUsersInRoleAsync("Coordinator").GetAwaiter().GetResult();
                bool coordinatorFound = false;
                foreach (var coordinator in usersInFaculty)
                {
                    if (usersAsCoordinator.Any(u => u.Id == coordinator.Id))
                    {
                        coordinatorFound = true; 
                        var emailTo = coordinator.Email;
                        if (emailTo == null)
                        {
                            TempData["error"] = "There currently no coordinator in faculty. Please check with admin.";
                            return View(contribution);
                        }
                        var emailSubject = "Please check the new submitted contribution.";
                        var emailBody = $"Please check the new submitted contribution made by {user.Email}.";
                        var emailComponent = new EmailComponent
                        {
                            To = emailTo,
                            Subject = emailSubject,
                            Body = emailBody
                        };
                        _emailService.SendEmailAsync(emailComponent).GetAwaiter().GetResult();
                    }
                }
                if (coordinatorFound)
                {
                    _unitOfWork.ContributionRepository.Add(contribution);
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "There currently no coordinator in faculty. Please check with admin.";
                    return View(contribution);
                }
            }
            else
            {
                TempData["error"] = "Please login";
                return View(contribution);
            }
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
