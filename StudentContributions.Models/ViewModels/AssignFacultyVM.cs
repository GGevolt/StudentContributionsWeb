using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
    public class AssignFacultyVM
    {
        public ApplicationUser User { get; set; }
        public List<Faculty> Faculties { get; set; }
    }
}
