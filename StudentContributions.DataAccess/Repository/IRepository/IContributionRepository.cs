using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IContributionRepository : IRepository<Contribution>
    {
        void Update(Contribution contribution);
        
    }
}
