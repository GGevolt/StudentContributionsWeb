using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class SemesterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SemesterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var semesters = _unitOfWork.SemesterRepository.GetAll().ToList();
            return View(semesters);
        }

        public IActionResult Create()
        {
            var latestSemester = _unitOfWork.SemesterRepository.GetAll()
                                    .OrderByDescending(s => s.EndDate).FirstOrDefault();
            var newSemester = new Semester
            {
                EndDate = DateTime.Today
            };

            if (latestSemester == null)
            {
                newSemester.StartDate = DateTime.Today;
            }
            else
            {
                newSemester.StartDate = latestSemester.EndDate.AddDays(1);
            }

            return View(newSemester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Semester semester)
        {
            ModelState.Remove("Magazines");
            if (semester.EndDate < semester.StartDate)
            {
                ModelState.AddModelError("EndDate", "EndDate must be greater than StartDate.");
            }
            if (semester.StartDate < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "StartDate cannot be in the past.");
            }
            var latestSemester = _unitOfWork.SemesterRepository.GetAll()
                                   .OrderByDescending(s => s.EndDate).FirstOrDefault();
            if (latestSemester != null && semester.StartDate <= latestSemester.EndDate)
            {
                ModelState.AddModelError("StartDate", "StartDate must be after the end date of the most recent semester.");
            }

            var activeSemesterExists = _unitOfWork.SemesterRepository.GetAll()
                                         .Any(s => s.IsActive);
            if (activeSemesterExists && semester.IsActive)
            {
                ModelState.AddModelError("IsActive", "Cannot set this semester as active because there is already an active semester.");
            }

            if (ModelState.IsValid)
            {
                semester.IsActive = false;
                _unitOfWork.SemesterRepository.Add(semester);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var semester = _unitOfWork.SemesterRepository.Get(s => s.ID == id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Semester semester)
        {
            ModelState.Remove("Magazines");
            if (semester.EndDate <= semester.StartDate)
            {
                ModelState.AddModelError("EndDate", "EndDate must be greater than StartDate.");
            }
            if (semester.StartDate < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "StartDate cannot be in the past.");
            }
            if (semester.IsActive)
            {

                var isActiveSemesterExists = _unitOfWork.SemesterRepository.GetAll()
                    .Any(s => s.IsActive && s.ID != semester.ID);

                if (isActiveSemesterExists)
                {
                    ModelState.AddModelError("IsActive", "There is already an active semester. Only one semester can be active at a time.");
                }
            }

            if (ModelState.IsValid)
            {
                var existingSemester = _unitOfWork.SemesterRepository.Get(s => s.ID == semester.ID);
                if (existingSemester != null)
                {
                    existingSemester.StartDate = semester.StartDate;
                    existingSemester.EndDate = semester.EndDate;
                    existingSemester.IsActive = semester.IsActive;

                    _unitOfWork.SemesterRepository.Update(existingSemester);
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            return View(semester);
        }




        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var semester = _unitOfWork.SemesterRepository.Get(c => c.ID == id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var semester = _unitOfWork.SemesterRepository.Get(c => c.ID == id);
            if (semester != null)
            {
                _unitOfWork.SemesterRepository.Remove(semester);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
