using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Utility.Interfaces;
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

        public ContributionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
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
        public IActionResult Create(Contribution contribution)
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
