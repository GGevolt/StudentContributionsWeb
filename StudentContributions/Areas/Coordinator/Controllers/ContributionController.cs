using Microsoft.AspNetCore.Mvc;

namespace StudentContributions.Areas.Coordinator.Controllers
{
    public class ContributionController : Controller
    {
        public IActionResult Index()
        {
            //edit contribtion
            return View();
        }
    }
}
