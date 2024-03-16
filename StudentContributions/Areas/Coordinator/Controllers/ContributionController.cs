using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.Coordinator.Controllers
{
    [Area("BasicUser")]
    public class ContributionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContributionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            if (ModelState.IsValid)
            {
                _unitOfWork.ContributionRepository.Add(contribution);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(contribution);
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
