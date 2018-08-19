using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllWithChildren(params Expression<Func<T, object>>[] includeProperties);
        T Get(long id);
        void Insert(T entity);
        void InsertRange(List<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteAll(string tableName);
    }
}
