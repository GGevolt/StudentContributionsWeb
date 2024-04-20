using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository.IRepository;
using System;
using System.Linq.Expressions;

namespace StudentContributions.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _dbContext;
        internal DbSet<T> dbSet { get; set; }
        public Repository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = _dbContext.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracker = false)
        {
            IQueryable<T> query = dbSet;
            if (tracker)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();

            }
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperty = null)
        {

            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperty))
            {
                query = query.Include(includeProperty);
            }
            return query.ToList();
        }

		public IEnumerable<T> GetAllMore(string? includeProperty = null, string? moreProperty = null)
		{

			IQueryable<T> query = dbSet;
			if (!string.IsNullOrEmpty(includeProperty))
			{
				query = query.Include(includeProperty);
			}
			if (!string.IsNullOrEmpty(moreProperty))
			{
				query = query.Include(moreProperty);
			}
			return query.ToList();
		}


		public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }


    }
}
