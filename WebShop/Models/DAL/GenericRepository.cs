using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebShop.Models.DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {
        DbContext Context;
        DbSet<T> dbSet;
        public GenericRepository(DbContext Context)
        {
            this.Context = Context;
            this.dbSet = Context.Set<T>();
        }
        public virtual T GetByID(object id)
        {
            return dbSet.Find(id);
        }
        public virtual void Insert(T entity)// при вставке хочется чтоб Id сам генерился 
        {
            dbSet.Add(entity);
        }
        public virtual IEnumerable<T> Get()
        {
            return dbSet.ToList();
        }
        public virtual void DeleteById(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(T entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public virtual void Update(T entityToUpdate)
        {
           
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}