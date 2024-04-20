using StudentContributions.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StudentContributions.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null);
        IEnumerable<T> GetAllMore(string? includeProperty = null, string? moreProperty = null);
		T Get(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracker = false);
        void Add(T entity);
        void Remove(T entity);
    }
}
