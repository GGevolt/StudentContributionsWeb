using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
    public class HomeTestVM
    {
        public List<Magazine> Magazines { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
    }
}
