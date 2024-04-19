using StudentContributions.Models.Models;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
        IEnumerable<ApplicationUser> getAllIncludeFaculty();
	}
}
