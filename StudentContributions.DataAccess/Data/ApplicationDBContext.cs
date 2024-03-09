using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StudentContributions.DataAccess.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Contribution> Contributions { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
