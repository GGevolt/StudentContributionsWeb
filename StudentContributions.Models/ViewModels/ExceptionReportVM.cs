using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.Models.ViewModels
{
	public class ExceptionReportVM
	{
		public IEnumerable<Contribution> NullComment {  get; set; }
		public IEnumerable<Contribution> NullCommentfor14day { get; set; }
	}
}
