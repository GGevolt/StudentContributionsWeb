﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Student.Controllers
{
    [Area("Student")]
    public class AnalysisController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View(_unitOfWork.FacultyRepository.GetAll().ToList());
        }
        public IActionResult AcademicYear(int id)
        {
            List<AcademicYearVM> academics = new List<AcademicYearVM>();
            var semesters = _unitOfWork.SemesterRepository.GetAll();
            foreach (var semester in semesters)
            {
                int contributionsInF = 0;
                int contributionsBySF = 0;
                int contributorsBySF = 0;

                var magazinesInF = _unitOfWork.MagazineRepository.GetAllWithContributions(id);
                if (magazinesInF != null)
                {
                    contributionsInF = magazinesInF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Count();
                }

                var magazinesInSF = magazinesInF != null ? magazinesInF.Where(m => m.SemesterID == semester.ID) : null;
                if (magazinesInSF != null)
                {
                    contributionsBySF = magazinesInSF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Count();
                    contributorsBySF = magazinesInSF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Select(c => c.UserID).Distinct().Count();
                }

                double percentage = contributionsInF != 0 ? (contributionsBySF / (double)contributionsInF) * 100 : 0;
                academics.Add(new AcademicYearVM
                {
                    semester = semester,
                    ContributionNum = contributionsBySF,
                    Percentage = percentage,
                    ContributorNum = contributorsBySF,
                    FacultyId = id
                });
            }
            return View(academics);
        }
    }
}
