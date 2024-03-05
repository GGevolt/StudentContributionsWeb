using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentContributions.Models.Models;

namespace StudentContributions.Utility.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(EmailComponent emailComponent);
    }
}
