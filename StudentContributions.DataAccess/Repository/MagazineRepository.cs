using Microsoft.EntityFrameworkCore;
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
    public class MagazineRepository : Repository<Magazine>, IMagazineRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public MagazineRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Magazine magazine)
        {
            _dbContext.Update(magazine);
        }
        public DateTime? GetClosureDate()
        {
            return _dbContext.Magazines.Select(m => m.ClosureDate).FirstOrDefault();
        }
        public Magazine GetById(int id)
        {
            
            return _dbContext.Magazines.Find(id);
        }
        public IEnumerable<Magazine> GetAllWithContributions(int facultyId)
        {
            return _dbContext.Magazines.Include(m => m.Contributions).Where(m => m.FacultyID == facultyId).ToList();
        }
        public IEnumerable<Magazine> MagazinesIncludeFacultySemester()
        {
            return _dbContext.Magazines.Include(m => m.Faculty).Include(m => m.Semester).ToList();
        }
    }
}
