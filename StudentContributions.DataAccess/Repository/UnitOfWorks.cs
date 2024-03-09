using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository.IRepository;

namespace StudentContributions.DataAccess.Repository
{
    public class UnitOfWorks : IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IMagazineRepository MagazineRepository { get; private set; }
        public IFacultyRepository FacultyRepository { get; private set; }
        public ISemesterRepository SemesterRepository { get; private set; }
        public IContributionRepository ContributionRepository { get; private set; }
        public UnitOfWorks(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
            ApplicationUserRepository = new ApplicationUserRepository(dBContext);
            MagazineRepository = new MagazineRepository(dBContext);
            FacultyRepository = new FacultyRepository(dBContext);
            SemesterRepository = new SemesterRepository(dBContext);
            ContributionRepository = new ContributionRepository(dBContext);

        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
