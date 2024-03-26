using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models.ViewModels;
using StudentContributions.Models.Models;
using System.Diagnostics;
using System.IO.Compression;
using StudentContributions.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

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

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user != null)
            {
                return View(_unitOfWork.MagazineRepository.GetAll().Where(m => m.FacultyID == user.FacultyID));
            }
            return View(_unitOfWork.MagazineRepository.GetAll());
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
