using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;
using StudentContributions.Utility.Interfaces;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
    //[Authorize(Roles = "Student,Coordinator")]
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
            var activeSemester = _unitOfWork.SemesterRepository.GetAll().ToList().FirstOrDefault(s => s.IsActive);
            var magazineClosureDate = activeSemester?.Magazines?.FirstOrDefault()?.ClosureDate;
            var semesterClosureDate = activeSemester?.EndDate;

            ViewBag.Timestamp1 = magazineClosureDate;
            ViewBag.Timestamp2 = semesterClosureDate;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var contributions = _unitOfWork.ContributionRepository.IncludeMaga().Where(c => c.UserID.Equals(userId)).ToList();
            return View(contributions);

        }

        public IActionResult Create(int? magID)
        {
            if (magID == null || magID == 0)
            {
                return NotFound();
            }
            var mag = _unitOfWork.MagazineRepository.Get(m => m.ID == magID);
            if (mag == null)
            {
                return NotFound();
            }
            Contribution con = new Contribution();
            con.MagazineID = (int) magID;
            con.SubmissionDate = DateTime.Now;
           
            return View(con);
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
                var usersInFaculty = _unitOfWork.ApplicationUserRepository.GetAll(u => u.FacultyID == facultyID).ToList();
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
                    var magazine = _unitOfWork.MagazineRepository.GetById(contribution.MagazineID);

                    if (magazine == null || DateTime.Now > magazine.ClosureDate)
                    {
                        ModelState.AddModelError("Error: ", "The contribution period for the selected magazine has ended.");
                        return View(contribution);
                    }
                    contribution.Contribution_Status = "Pending";
                    _unitOfWork.ContributionRepository.Add(contribution);
                    _unitOfWork.Save();

                    string uploadPath = Path.Combine(this._webHost.WebRootPath, "Contributions", contribution.ID.ToString());
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    if (files != null)
                    {
                        foreach (var filecheck in files)
                        {
                            var permittedExtensions = new[] { ".jpg", ".png", ".jpeg", ".doc", ".docx" };
                            var extension = Path.GetExtension(filecheck.FileName).ToLowerInvariant();

                            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                            {
                                TempData["error"] = "You have chosen invalid file type, please choose again";
                                return View(contribution);
                            }
                        }
                        foreach (var file in files)
                        {
                          string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                          using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                          {
                              file.CopyTo(fileStream);
                          }
                        }
                    }
                    
                    return RedirectToAction("Details", new {id = contribution.ID});
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
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            details.IsSubmitPerson = user != null && user.Id == contribution.UserID;
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

            if (files != null)
            {
                foreach (var filecheck in files)
                {
                    var permittedExtensions = new[] { ".jpg", ".png", ".jpeg", ".doc", ".docx" };
                    var extension = Path.GetExtension(filecheck.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        TempData["error"] = "File chosen must be.pdf,.doc,.docx,.jpg,.jpeg,.png";
                        return RedirectToAction("Details", new { id = id });
                    }
                }
                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
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
            var contri = _unitOfWork.ContributionRepository.Get(c=>c.ID == id);
            var SemesID = _unitOfWork.MagazineRepository.Get(m=>m.ID== contri.MagazineID).SemesterID;
            var Semes = _unitOfWork.SemesterRepository.Get(s=>s.ID==SemesID);
            if (Semes.IsActive== false || Semes == null)
            {
                TempData["error"] = "The semester is not active or don't exist.";
                return RedirectToAction(nameof(Index));
            }
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id);

            if (contribution == null || DateTime.Now > Semes.EndDate)
            {
                TempData["error"] = "The editing period has ended or the contribution does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(contribution);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contribution contribution)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            contribution.UserID = user.Id;
            _unitOfWork.ContributionRepository.Update(contribution);
            _unitOfWork.Save();
            return View(contribution);
        }
        [Authorize(Roles = "Student")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var activeSemester = _unitOfWork.SemesterRepository.GetAll().ToList().FirstOrDefault(s => s.IsActive);
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id);

            if (contribution == null || activeSemester == null || DateTime.Now > activeSemester.EndDate)
            {
                TempData["error"] = "The deletion period has ended or the contribution does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(contribution);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public IActionResult DeleteConfirmed(int id)
        {
            var activeSemester = _unitOfWork.SemesterRepository.GetAll().ToList().FirstOrDefault(s => s.IsActive);
            if (activeSemester == null || DateTime.Now > activeSemester.EndDate)
            {
                TempData["error"] = "The deletion period has ended.";
                return RedirectToAction(nameof(Index));
            }

            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id);
            if (contribution != null)
            {
                _unitOfWork.ContributionRepository.Remove(contribution);
                _unitOfWork.Save();
                string path = Path.Combine(this._webHost.WebRootPath, "Contributions", id.ToString());
                var confolder = new DirectoryInfo(path);
                confolder.Delete(true);
                TempData["success"] = "Contribution deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Contribution not found.";
            return NotFound();

        }
    }
}
