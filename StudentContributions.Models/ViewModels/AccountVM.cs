using Microsoft.AspNetCore.Identity;
using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
    public class AccountVM
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
