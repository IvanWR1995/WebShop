using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models.DAL
{
    public  interface IUnitOfWork
    {
        IGenericRepository<Customer> GetCustomers {get;}
        IGenericRepository<Order> GetOrders { get; }
        IGenericRepository<ElementOfOrder> GetElementOfOrders { get; }
        IGenericRepository<Item> GetItems { get; }
        void Save();
        void Dispose();
        
    }
}
