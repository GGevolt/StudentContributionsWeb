using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.BasicUser.Controllers
{
    [Area("BasicUser")]
    public class MagazineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MagazineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var magazines = _unitOfWork.MagazineRepository.GetAll();
            return View(magazines);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Magazine magazine)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.MagazineRepository.Add(magazine);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(magazine);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var magazine = _unitOfWork.MagazineRepository.Get(m => m.ID == id);
            if (magazine == null)
            {
                return NotFound();
            }
            return View(magazine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Magazine magazine)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.MagazineRepository.Update(magazine);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(magazine);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var magazine = _unitOfWork.MagazineRepository.Get(m => m.ID == id);
            if (magazine == null)
            {
                return NotFound();
            }
            return View(magazine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var magazine = _unitOfWork.MagazineRepository.Get(m => m.ID == id);
            if (magazine != null)
            {
                _unitOfWork.MagazineRepository.Remove(magazine);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
