using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
		[ForeignKey("FacultyID")]
		[ValidateNever]
		public Faculty Faculty { get; set; }

        public int? FacultyID { get; set; }
        public virtual ICollection<Contribution> Contributions { get; set; }
    }
}
