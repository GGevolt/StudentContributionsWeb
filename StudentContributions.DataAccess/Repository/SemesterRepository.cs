using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.DataAccess.Repository
{
    public class SemesterRepository : Repository<Semester>, ISemesterRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public SemesterRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Semester semester)
        {
            _dbContext.Update(semester);
        }

        public DateTime? GetClosureDate()
        {
            return _dbContext.Semesters.Select(s => s.EndDate).FirstOrDefault();
        }
    }
}
