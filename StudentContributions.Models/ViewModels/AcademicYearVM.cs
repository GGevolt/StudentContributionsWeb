using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
    public class AcademicYearVM
    {
       public Semester semester {  get; set; }
       public double Percentage { get; set; }
       public int ContributionNum { get; set; }
       public int ContributorNum { get; set; }
       public int FacultyId { get; set; }
    }
}
