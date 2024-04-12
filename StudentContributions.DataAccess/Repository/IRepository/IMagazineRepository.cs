using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IMagazineRepository : IRepository<Magazine>
    {
        void Update(Magazine magazine);
        DateTime? GetClosureDate();
        Magazine GetById(int id);
        IEnumerable<Magazine> GetAllWithContributions(int facultyId);
    }
}
