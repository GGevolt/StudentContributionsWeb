using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.Models
{
    public class Contribution
    {
        [Key]
        public int ID { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string? Comment { get; set; }
        public string Contribution_Status { get; set; } = "Pending";
        [ForeignKey("MagazineID")]
        [ValidateNever]
        public Magazine Magazine { get; set; }
        
        public int MagazineID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string UserID { get; set; }

    }
}
