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
            var semesters = _unitOfWork.SemesterRepository.GetAll();
            return View(semesters);
        }

        public IActionResult Create()
        {
            var latestSemester = _unitOfWork.SemesterRepository.GetAll().OrderByDescending(s => s.EndDate).FirstOrDefault();
            var newSemester = new Semester();
            if (latestSemester != null)
            {
                // Đề xuất StartDate là ngày tiếp theo của EndDate semester gần nhất
                newSemester.StartDate = latestSemester.EndDate.AddDays(1);
            }

            return View(newSemester); // Truyền đối tượng newSemester với StartDate được đề xuất vào View
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Semester semester)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có semester nào đang active không
                var activeSemester = _unitOfWork.SemesterRepository.GetAll().FirstOrDefault(s => s.IsActive);
                if (activeSemester != null)
                {
                    // Nếu có, semester mới không được active
                    semester.IsActive = false;
                }
                else
                {
                    // Nếu không, đây là semester đầu tiên hoặc không có semester nào khác đang active
                    semester.IsActive = true;
                }
                var latestSemester = _unitOfWork.SemesterRepository.GetAll().OrderByDescending(s => s.EndDate).FirstOrDefault();
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
