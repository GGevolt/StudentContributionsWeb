using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "Admin")]
	public class MagazineController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public MagazineController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var magazines = _unitOfWork.MagazineRepository.MagazinesIncludeFacultySemester().ToList();
			return View(magazines);
		}

		public IActionResult Create()
		{
			PopulateFacultyAndSemesterLists();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Magazine magazine)
		{
			if (!ModelState.IsValid)
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
			if (!ModelState.IsValid)
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
			ViewBag.FacultyList = _unitOfWork.FacultyRepository.GetAll().ToList().Select(f => new SelectListItem { Value = f.ID.ToString(), Text = $"{f.ID}. {f.Name}" });
			ViewBag.SemesterList = _unitOfWork.SemesterRepository.GetAll().ToList().Select(s => new SelectListItem { Value = s.ID.ToString(), Text = $"{s.ID}. {s.StartDate.ToShortDateString()} - {s.EndDate.ToShortDateString()}" });
		}
	}
}
