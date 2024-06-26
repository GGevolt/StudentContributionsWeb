﻿using Microsoft.AspNetCore.Authorization;
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
            var newSemester = new Semester
            {
                EndDate = DateTime.Today,
                StartDate= DateTime.Today
            };
            return View(newSemester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Semester semester)
        {
            ModelState.Remove("Magazines");
            ValidateSemester(semester);
            
            

            if (ModelState.IsValid)
            {
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
            if (semester.EndDate < semester.StartDate)
            {
                ModelState.AddModelError("EndDate", "EndDate must be greater than StartDate.");
            }
            _unitOfWork.SemesterRepository.Update(semester);
            if (ModelState.IsValid)
            {
                

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

        private void ValidateSemester(Semester semester)
        {
            if (semester.EndDate < semester.StartDate)
                ModelState.AddModelError("EndDate", "EndDate must be greater than StartDate.");

            if (semester.StartDate < DateTime.Today)
                ModelState.AddModelError("StartDate", "StartDate cannot be in the past.");
        }
    }
}