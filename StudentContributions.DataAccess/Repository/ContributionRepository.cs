using Microsoft.EntityFrameworkCore;
using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.DataAccess.Repository
{
    public class ContributionRepository : Repository<Contribution>, IContributionRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ContributionRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Contribution contribution)
        {
            _dbContext.Update(contribution);
        }
        public IEnumerable<Contribution> IncludeUserToCon(IEnumerable<Contribution> contributions)
        {
            var contributionIds = contributions.Select(c => c.ID).ToList();
            return _dbContext.Contributions.Include(c => c.ApplicationUser).Where(c => contributionIds.Contains(c.ID)).ToList();
        }
        public IEnumerable<Contribution> IncludeMaga()
        {
            return _dbContext.Contributions.Include(c => c.Magazine).ToList();
        }
    }
}
