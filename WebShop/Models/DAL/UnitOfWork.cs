using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace WebShop.Models.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        DbContext Context;
        IGenericRepository<Customer> Customers;
        IGenericRepository<Order> Orders;
        IGenericRepository<ElementOfOrder> ElementOfOrders;
        IGenericRepository<Item> Items;
        public UnitOfWork(DbContext Context)
        {
            this.Context = Context;
        }
        public IGenericRepository<Customer> GetCustomers
        {
            get
            {
                if (this.Customers == null)
                    this.Customers = new GenericRepository<Customer>(this.Context);
                return this.Customers;
            }

        }
        public IGenericRepository<Order> GetOrders
        {
            get
            {
                if (this.Orders == null)
                    this.Orders = new GenericRepository<Order>(this.Context);
                return this.Orders;
            }

        }
        public IGenericRepository<ElementOfOrder> GetElementOfOrders
        {
            get
            {
                if (this.ElementOfOrders == null)
                    this.ElementOfOrders = new GenericRepository<ElementOfOrder>(this.Context);
                return this.ElementOfOrders;
            }
        }
        public IGenericRepository<Item> GetItems
        {
            get {
                if (this.Items == null)
                    this.Items = new GenericRepository<Item>(Context);
                return this.Items; }
        }
        public void Save()
        {
            Context.SaveChanges();
        }
        public void Dispose()
        {
            if(Context != null)
                Context.Dispose();
        }

    }
}