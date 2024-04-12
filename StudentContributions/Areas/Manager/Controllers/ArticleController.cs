using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ArticlesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArticlesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var contributions = _unitOfWork.ContributionRepository.GetAll().ToList();
            return View(contributions);
        }
    }
}
