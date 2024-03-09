using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.BasicUser.Controllers
{
    [Area("BasicUser")]
    public class FacultyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FacultyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Add(faculty);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View();

        }
        public IActionResult Edit(int? idb)
        {
            if (idb == null || idb == 0)
            {
                return NotFound();
            }
            Faculty? faculty = _unitOfWork.FacultyRepository.Get(c => c.ID == idb);

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
                return RedirectToAction("Index");
            }

            return View();
        }
        public IActionResult Delete(int? idb)
        {
            if (idb == null || idb == 0)
            {
                return NotFound();
            }
            Faculty? faculty = _unitOfWork.FacultyRepository.Get(c => c.ID == idb);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Faculty faculty)
        {
            _unitOfWork.FacultyRepository.Remove(faculty);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
