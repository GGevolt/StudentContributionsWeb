﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Ensure this is uncommented to restrict access
    public class MagazineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MagazineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public IActionResult Index()
		{
			var magazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Faculty", moreProperty: "Semester").ToList();
			return View(magazines);
		}
        public IActionResult Create()
        {
            PopulateFacultyAndSemesterLists();
            var model = new Magazine
            {
                ClosureDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Magazine magazine)
        {
            if (!ValidateClosureDate(magazine.ClosureDate, magazine.SemesterID))
            {
                PopulateFacultyAndSemesterLists();
                return View(magazine);
            }

            _unitOfWork.MagazineRepository.Add(magazine);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue || id <= 0)
            {
                return NotFound();
            }

            var magazine = _unitOfWork.MagazineRepository.Get(c => c.ID == id);
            if (magazine == null)
            {
                return NotFound();
            }

            PopulateFacultyAndSemesterLists();
            return View(magazine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Magazine magazine)
        {
            var originalMagazine = _unitOfWork.MagazineRepository.Get(m => m.ID == magazine.ID);
            if (originalMagazine == null)
            {
                return NotFound();
            }

            
            magazine.SemesterID = originalMagazine.SemesterID;

            if (!ValidateClosureDate(magazine.ClosureDate, magazine.SemesterID))
            {
                PopulateFacultyAndSemesterLists();
                return View(magazine);
            }

            _unitOfWork.MagazineRepository.Update(magazine);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id <= 0)
            {
                return NotFound();
            }

            var magazine = _unitOfWork.MagazineRepository.Get(c => c.ID == id);
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
            var magazine = _unitOfWork.MagazineRepository.Get(c => c.ID == id);
            if (magazine != null)
            {
                _unitOfWork.MagazineRepository.Remove(magazine);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private void PopulateFacultyAndSemesterLists()
        {
            ViewBag.FacultyList = _unitOfWork.FacultyRepository.GetAll().ToList().Select(f => new SelectListItem { Value = f.ID.ToString(), Text = $"{f.Name}" });
            ViewBag.SemesterList = _unitOfWork.SemesterRepository.GetAll(s => s.IsActive == true).ToList().Select(s => new SelectListItem { Value = s.ID.ToString(), Text = $"{s.SemesterName}: {s.StartDate.ToShortDateString()} - {s.EndDate.ToShortDateString()}" });
        }

        private bool ValidateClosureDate(DateTime closureDate, int semesterId)
        {
            var semester = _unitOfWork.SemesterRepository.Get(s => s.ID == semesterId);
            if (semester == null || semester.IsActive != true)
            {
                ModelState.AddModelError("SemesterID", "Selected semester does not exist or inactive.");
                return false;
            }

            if (closureDate < DateTime.Now)
            {
                ModelState.AddModelError("ClosureDate", "Closure date cannot be earlier than the current date.");
                return false;
            }

            if (closureDate < semester.StartDate || closureDate > semester.EndDate)
            {
                ModelState.AddModelError("ClosureDate", "Closure date must be within the selected semester's range.");
                return false;
            }

            return true;
        }
    }
}
