using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Faculty Faculty { get; set; }

        public int? FacultyID { get; set; }
        public virtual ICollection<Contribution>? Contributions { get; set; }
    }
}
