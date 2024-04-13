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
            var latestSemester = _unitOfWork.SemesterRepository.GetAll().ToList().OrderByDescending(s => s.EndDate).FirstOrDefault();
            var newSemester = new Semester();
            if (latestSemester != null)
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
            if (ModelState.IsValid)
            {
               
                var activeSemester = _unitOfWork.SemesterRepository.GetAll().ToList().FirstOrDefault(s => s.IsActive);
                if (activeSemester != null)
                {
                   
                    semester.IsActive = false;
                }
                else
                {
                   
                    semester.IsActive = true;
                }
                var latestSemester = _unitOfWork.SemesterRepository.GetAll().ToList().OrderByDescending(s => s.EndDate).FirstOrDefault();
                if (latestSemester != null && semester.StartDate <= latestSemester.EndDate)
                {
                    ModelState.AddModelError("StartDate", "StartDate phải sau ngày kết thúc của semester gần nhất.");
                }

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
            var semester = _unitOfWork.SemesterRepository.Get(c => c.ID == id);
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
            if (ModelState.IsValid)
            {
                _unitOfWork.SemesterRepository.Update(semester);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
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
