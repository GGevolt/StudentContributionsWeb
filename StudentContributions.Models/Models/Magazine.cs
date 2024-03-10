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
    public class Magazine
    {
        [Key]
        public int ID { get; set; }
        public string MagazineName { get; set; }
        public DateTime ClosureDate { get; set; }
        public virtual ICollection<Contribution> Contributions { get; set; }
        [ForeignKey("FacultyID")]
        [ValidateNever]
        public Faculty Faculty { get; set; }
        public int FacultyID { get; set; }
        [ForeignKey("SemesterID")]
        [ValidateNever]
        public Semester Semester { get; set; }
        public int SemesterID { get; set; }

    }
}
