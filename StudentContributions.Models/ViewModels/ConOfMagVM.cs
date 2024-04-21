using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
    public class ConOfMagVM
    {
        public Magazine Magazine { get; set; }
        public IEnumerable<Contribution> Contributions { get; set;}
        public bool SubmitStarted { get; set; }
    }
}
