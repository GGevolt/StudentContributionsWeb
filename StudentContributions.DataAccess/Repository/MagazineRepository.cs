﻿using Microsoft.EntityFrameworkCore;
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
    }
}
