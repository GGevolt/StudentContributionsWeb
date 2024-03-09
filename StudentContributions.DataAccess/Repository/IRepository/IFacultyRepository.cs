using StudentContributions.Models.Models;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IFacultyRepository : IRepository<Faculty>
    {
        void Update(Faculty faculty);
    }
}
