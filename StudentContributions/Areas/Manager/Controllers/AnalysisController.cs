using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Models.ViewModels;

namespace StudentContributions.Areas.Manager.Controllers
{
    [Area("Manager")]
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
                int approvedContributionsInF = 0;
                int approvedContributionsBySF = 0;
                var magazinesInF = _unitOfWork.MagazineRepository.GetAllWithContributions(id);
                if (magazinesInF != null)
                {
                    contributionsInF = magazinesInF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Count();
                    approvedContributionsInF = magazinesInF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Where(c=>c.Contribution_Status == "Approved").Count(); ;
                }

                var magazinesInSF = magazinesInF != null ? magazinesInF.Where(m => m.SemesterID == semester.ID) : null;
                if (magazinesInSF != null)
                {
                    contributionsBySF = magazinesInSF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Count();
                    approvedContributionsBySF = magazinesInSF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Where(c => c.Contribution_Status == "Approved").Count();
                    contributorsBySF = magazinesInSF.Where(m => m.Contributions != null).SelectMany(m => m.Contributions).Select(c => c.UserID).Distinct().Count();
                }

                double percentageOfContribution = contributionsInF != 0 ? (contributionsBySF / (double)contributionsInF) * 100 : 0;
                double percentageOfApproved = approvedContributionsInF != 0 ? (approvedContributionsBySF / (double)approvedContributionsInF) * 100 : 0;
                academics.Add(new AcademicYearVM
                {
                    semester = semester,
                    ContributionNum = contributionsBySF,
                    ApprovedContributionNum = approvedContributionsBySF,
                    PercentageOfContribution = percentageOfContribution,
                    PercentageOfApproved = percentageOfApproved,
                    ContributorNum = contributorsBySF,
                    FacultyId = id
                });
            }
            return View(academics);
        }
		public IActionResult ExceptionReport(int id)
        {
			var magazinesInF = _unitOfWork.MagazineRepository.GetAllWithContributions(id);
            var contribution = magazinesInF.Where(m=>m.Contributions !=null).SelectMany(m=>m.Contributions).ToList();
			ExceptionReportVM ERvm = new ExceptionReportVM
			{
				NullComment = _unitOfWork.ContributionRepository.IncludeUserToCon(contribution.Where(c => c.Comment == null)),
                NullCommentfor14day = _unitOfWork.ContributionRepository.IncludeUserToCon(contribution.Where(c => c.Comment == null && DateTime.Now > c.SubmissionDate.AddDays(14)))
			};
			return View(ERvm);
        }
	}
}

