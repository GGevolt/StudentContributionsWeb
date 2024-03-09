namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUserRepository { get; }
        IFacultyRepository FacultyRepository { get; }
        ISemesterRepository SemesterRepository { get; }
        IContributionRepository ContributionRepository { get; }
        IMagazineRepository MagazineRepository { get; }
        void Save();
    }
}
