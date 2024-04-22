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


        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        [HttpGet]
        public IActionResult Index(string? search, int currentPage = 1)
        {
            int pageSize = 8;
            HomeTestVM homeTestVM = new HomeTestVM();

            // Get the current user
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();

            // Retrieve all magazines or filter based on the user's role
            if (user != null && _userManager.IsInRoleAsync(user, "Coordinator").GetAwaiter().GetResult())
            {
                // Filter the magazines based on the FacultyID
                homeTestVM.Magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty")
                    .Where(m => m.FacultyID == user.Faculty.ID)
                    .ToList();
            }
            else
            {
                // Retrieve all magazines
                homeTestVM.Magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty")
                    .ToList();
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                homeTestVM.Magazines = homeTestVM.Magazines
                    .Where(m => m.MagazineName.ToLower().Contains(search.ToLower()))
                    .ToList();
                homeTestVM.Search = search;
            }

            var totalRecords = homeTestVM.Magazines.Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            // Apply pagination
            homeTestVM.Magazines = homeTestVM.Magazines
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            homeTestVM.CurrentPage = currentPage;
            homeTestVM.TotalPages = totalPages;
            homeTestVM.PageSize = pageSize;
            homeTestVM.Semesters = _unitOfWork.SemesterRepository.GetAll().ToList();
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
