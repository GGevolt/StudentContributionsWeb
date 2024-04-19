using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var userRoles = new List<AccountVM>();
            var users = _unitOfWork.ApplicationUserRepository.getAllIncludeFaculty();
            foreach (var user in users)
            {
                List<string> roles = (List<string>)_userManager.GetRolesAsync(user).GetAwaiter().GetResult();
                userRoles.Add(new AccountVM { User = user, Roles = roles });
            }

            return View(userRoles);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string newrole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
            var result = await _userManager.AddToRoleAsync(user, newrole);
            if (result.Succeeded)
            {
                TempData["success"] = "Role assigned successfully";
            }
            else
            {
                TempData["error"] = "Failed to assign role";
            }
            return RedirectToAction("Index");
        }
       
        public IActionResult AssignFaculty(string userId)
        {
            AssignFacultyVM assign = new AssignFacultyVM() { 
                User = _unitOfWork.ApplicationUserRepository.Get(u=>u.Id == userId),
                Faculties = _unitOfWork.FacultyRepository.GetAll().ToList()
            };
            return View(assign);
        }
        [HttpPost]
        public IActionResult AssignFaculty(string userID, int FacultyID)
        {
            var user = _userManager.FindByIdAsync(userID).GetAwaiter().GetResult();
            if(user!=null)
            {
                user.FacultyID = FacultyID;
                _userManager.UpdateAsync(user).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
    }
}
