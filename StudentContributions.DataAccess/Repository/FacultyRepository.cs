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
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public FacultyRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Faculty faculty)
        {
            _dbContext.Update(faculty);
        }
    }
}
