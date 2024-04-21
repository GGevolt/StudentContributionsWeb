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
            homeTestVM.Magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty").OrderByDescending(m => m.ClosureDate).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                homeTestVM.Magazines = homeTestVM.Magazines.Where(m => m.MagazineName.ToLower().Contains(search.ToLower())).ToList();
                homeTestVM.Search = search;
            }
            var totalRecords = homeTestVM.Magazines.Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;
            homeTestVM.Magazines = homeTestVM.Magazines.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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
