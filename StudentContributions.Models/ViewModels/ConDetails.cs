using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentContributions.Models;
using StudentContributions.Models.Models;

namespace StudentContributions.Models.ViewModels
{
    public class ConDetails
    {
        public Contribution Contribution { get; set; }
        public List<string> Filenames { get; set; }
    }
}
