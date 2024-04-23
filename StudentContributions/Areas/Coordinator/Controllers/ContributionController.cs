using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace StudentContributions.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    [Authorize(Roles = "Coordinator")]
    public class ContributionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHost;

        public ContributionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userFacultyId = user.FacultyID;

            var contributions = _unitOfWork.ContributionRepository.GetAll(includeProperty: "Magazine.Semester")
                               .Where(c => c.Magazine.FacultyID == userFacultyId &&
                                           c.Contribution_Status.Contains("Pending") &&
                                           ((DateTime.Now - c.SubmissionDate).Days < 14) &&
                                           DateTime.Now < c.Magazine.Semester.EndDate);

            return View(contributions);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id, includeProperty: "Magazine");
            if (contribution == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var userFacultyId = user.FacultyID;

            if (contribution.Magazine.FacultyID != userFacultyId)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index");
            }

            ConDetails conForm = new ConDetails();
            conForm.Contribution = contribution;
            conForm.Filenames = new List<string>();
            string path = Path.Combine(this._webHost.WebRootPath, "Contributions", conForm.Contribution.ID.ToString());
            if (Directory.Exists(path))
            {
                string[] paths = Directory.GetFiles(Path.Combine(_webHost.WebRootPath, "Contributions", contribution.ID.ToString()));
                foreach (string file in paths)
                {
                    conForm.Filenames.Add(Path.GetFileName(file));
                }
            }

            return View(conForm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConDetails conForm, List<IFormFile>? files)
        {
            var user = await _userManager.GetUserAsync(User);
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == conForm.Contribution.ID, includeProperty: "Magazine");
            if (contribution == null)
            {
                return NotFound();
            }
            if (user == null || contribution.Magazine.FacultyID != user.FacultyID)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index");
            }

            if (files != null)
            {
                foreach (var filecheck in files)
                {
                    var permittedExtensions = new[] { ".jpg", ".png", ".jpeg", ".doc", ".docx", ".pdf" };
                    var extension = Path.GetExtension(filecheck.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        TempData["error"] = "File chosen must be.pdf,.doc,.docx,.jpg,.jpeg,.png";
                        return RedirectToAction("Edit", new { id = conForm.Contribution.ID });
                    }
                }
            }
                

            _unitOfWork.ContributionRepository.Update(conForm.Contribution);
            _unitOfWork.Save();

            string uploadPath = Path.Combine(_webHost.WebRootPath, "Contributions", conForm.Contribution.ID.ToString());
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            if (files != null)
            {
                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }

            TempData["success"] = "Contribution updated successfully.";
            return RedirectToAction("Edit", new {id = conForm.Contribution.ID});
        }

        public FileResult DownloadFile(string fileName, int id)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString() + "/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> DeleteFile(string fileName, int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id, includeProperty: "Magazine");

            if (user == null || contribution.Magazine.FacultyID != user.FacultyID)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index");
            }

            string path = Path.Combine(_webHost.WebRootPath, "Contributions", id.ToString() + "/") + fileName;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }

            return RedirectToAction("Edit", new { id = id });
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


        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            if (newStatus != "Approved" && newStatus != "Deny")
            {
                TempData["error"] = "Invalid status.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var userFacultyId = user.FacultyID;

            var contribution = _unitOfWork.ContributionRepository.Get(c => c.ID == id, includeProperty: "Magazine");

            if (user == null || contribution.Magazine.FacultyID != user.FacultyID)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index");
            }

            if (contribution == null || (DateTime.Now - contribution.SubmissionDate).Days > 14)
            {
                TempData["error"] = "Contribution not found, is too old to change status, or you do not have permission.";
                return RedirectToAction("Index");
            }

            if (contribution.Comment == null)
            {
                TempData["error"] = "Must comment on contribution before decision.";
                return RedirectToAction("Index");
            }

            contribution.Contribution_Status = newStatus;
            _unitOfWork.ContributionRepository.Update(contribution);
            _unitOfWork.Save();
            TempData["success"] = "Status changed successfully.";

            return RedirectToAction("Index");
        }


    }
}
