using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var userRoles = new List<AccountVM>();
            var users = _userManager.Users.ToList();
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
    }
}
