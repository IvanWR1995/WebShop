using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models.DAL
{
    public interface IGenericRepository<T> where T : class
    {
        T GetByID(object id);
        void Insert(T entity);
        IEnumerable<T> Get();
        void DeleteById(object id);
        void Delete(T entityToDelete);
        void Update(T entityToUpdate);
        
    }
}
