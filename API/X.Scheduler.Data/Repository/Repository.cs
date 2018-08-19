using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationContext context;
        private DbSet<T> entities;

        public Repository(ApplicationContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public T Get(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        /// <summary>
        /// TODO: Fix this method
        /// </summary>
        /// <param name="entities"></param>
        public void InsertRange(List<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            entities.AddRange(entities);
            context.AttachRange(entities);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public void DeleteAll(string tableName)
        {
            var sql = $"TRUNCATE TABLE [{tableName}]";
            context.Database.ExecuteSqlCommand(sql);

            // var sql = @"TRUNCATE TABLE @TableName";
            //context.Database.ExecuteSqlCommand(sql, new SqlParameter("@TableName", tableName));
        }

        public IEnumerable<T> GetAllWithChildren(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsEnumerable();
        }

    }
}
