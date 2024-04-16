using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
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

        public ContributionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userFacultyId = user.FacultyID;

            var contributions = _unitOfWork.ContributionRepository.GetAll(includeProperty: "Magazine")
                               .Where(c => c.Magazine.FacultyID == userFacultyId)
                               .ToList();

            return View(contributions);
        }

        public IActionResult Edit(int id)
        {
            var contribution = _unitOfWork.ContributionRepository.GetAll(c => c.ID == id, includeProperty: "Magazine")
                                       .FirstOrDefault();  
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contribution contribution)
        {
            if (id != contribution.ID)  
            {
                return NotFound();
            }
            _unitOfWork.ContributionRepository.Update(contribution);
            _unitOfWork.Save();
            TempData["success"] = "Contribution updated successfully.";
            return RedirectToAction(nameof(Index));
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

            var contribution = _unitOfWork.ContributionRepository.GetAll(c => c.ID == id, includeProperty: "Magazine")
                                        .FirstOrDefault();

            if (contribution == null || (DateTime.Now - contribution.SubmissionDate).Days > 14)
            {
                TempData["error"] = "Contribution not found, is too old to change status, or you do not have permission.";
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
