using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface ISemesterRepository : IRepository<Semester>
    {
        void Update(Semester semester);
    }
}
