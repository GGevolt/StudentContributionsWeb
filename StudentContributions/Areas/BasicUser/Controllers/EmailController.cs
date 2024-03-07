using Microsoft.AspNetCore.Mvc;
using StudentContributions.Models.Models;
using StudentContributions.Utility.Interfaces;
using System.Text.Encodings.Web;

namespace StudentContributions.Areas.BasicUser.Controllers
{
    //Class de tess thoi, ko co lam gi het nghe
    [ApiController]
    [Route("[controller]")]
    public class EmailController : Controller
    {
        public readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] EmailComponent emailComponent)
        {
            try
            {
                await _emailService.SendEmailAsync(emailComponent);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
