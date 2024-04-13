using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class FacultyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var faculties = _unitOfWork.FacultyRepository.GetAll().ToList();
            return View(faculties);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Faculty faculty)
        {
            if (!ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Add(faculty);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var faculty = _unitOfWork.FacultyRepository.Get(c => c.ID == id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Update(faculty);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var faculty = _unitOfWork.FacultyRepository.Get(c => c.ID == id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var faculty = _unitOfWork.FacultyRepository.Get(c => c.ID == id);
            if (faculty != null)
            {
                _unitOfWork.FacultyRepository.Remove(faculty);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
