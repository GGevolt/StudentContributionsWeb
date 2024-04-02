using Microsoft.EntityFrameworkCore;
using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;

namespace StudentContributions.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ApplicationUserRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }
        public void Update(ApplicationUser applicationUser)
        {
            _dbContext.Update(applicationUser);
        }
    }
}
