using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models.ViewModels;
using StudentContributions.Models.Models;
using System.Diagnostics;
using System.IO.Compression;
using StudentContributions.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHost;


        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHost = webHost;
        }

        [HttpGet]
        public IActionResult Index(string? search, int currentPage = 1)
        {
            int pageSize = 8;
            HomeTestVM homeTestVM = new HomeTestVM();
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();

            if (user != null && _userManager.IsInRoleAsync(user, "Coordinator").GetAwaiter().GetResult())
            {
                homeTestVM.Magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty", moreProperty: "Semester")
                    .Where(m => m.FacultyID == user.Faculty.ID)
                    .OrderByDescending(m => m.ClosureDate)
                    .ToList();
            }
            else
            {
                homeTestVM.Magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty", moreProperty: "Semester")
                    .OrderByDescending(m => m.ClosureDate)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                homeTestVM.Magazines = homeTestVM.Magazines
                    .Where(m => m.MagazineName.ToLower().Contains(search.ToLower()))
                    .OrderByDescending(m => m.ClosureDate)
                    .ToList();
                homeTestVM.Search = search;
            }

            var totalRecords = homeTestVM.Magazines.Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            homeTestVM.Magazines = homeTestVM.Magazines
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            homeTestVM.CurrentPage = currentPage;
            homeTestVM.TotalPages = totalPages;
            homeTestVM.PageSize = pageSize;
            return View(homeTestVM);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var magazine = _unitOfWork.MagazineRepository.Get(c => c.ID == id, includeProperty: "Semester");
            if (magazine == null)
            {
                return NotFound();
            }
            ConOfMagVM conOfMagVM = new ConOfMagVM();
            if (magazine.Semester.StartDate > DateTime.Now || DateTime.Now > magazine.ClosureDate)
            {
                conOfMagVM.SubmitStarted = false;
            }
            else
            {
                conOfMagVM.SubmitStarted = true;
            }
            conOfMagVM.Magazine = magazine;
            var contributions = _unitOfWork.ContributionRepository.GetAll(c => c.MagazineID == id && c.Contribution_Status.Contains("Approved"));
            //var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            //if (user == null)
            //{
            //  contributions = contributions.Where(c => c.Contribution_Status.Contains("Approved"));
            //}
            //else if (_userManager.IsInRoleAsync(user, "Student").GetAwaiter().GetResult())
            //{
            //    contributions = contributions.Where(c => c.Contribution_Status.Contains("Approved") && c.UserID.Equals(user.Id));
            //}
            conOfMagVM.Contributions = contributions;
            return View(conOfMagVM);
        }

        public IActionResult DownloadZipMagazine(int? id)
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
                        zipArchive.CreateEntryFromFile(file, foldername + "/" + Path.GetFileName(file));
                    }
                }

            }

            memoryStream.Position = 0;

            var mag = _unitOfWork.MagazineRepository.Get(m => m.ID == id);
            string filename = "Magazine_" + id.ToString() + "_" + mag.MagazineName + ".zip";
            return File(memoryStream, "application/zip", filename);
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
