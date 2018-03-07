namespace WebShop.Models
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
  /*  public class ShopContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ElementOfOrder> ElementsOfOrders { get; set; }
        public DbSet<Item> Items { get; set; }
        
        public ShopContext()
            : base("name=ShopContext")
        {
            
        }

        
    }
    */
    public class Customer
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public string Address { get; set; }

        public double? Discount { get; set; }
        public ICollection<Order> CollectionOrders { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }//FK
        public Customer CurrentCustomer { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; }
        public ICollection<ElementOfOrder> ElementOfOrders { get; set; }

    }
    public class ElementOfOrder
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order CurrentOrder { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCount { get; set; }
        public decimal ItemPrice { get; set; }
        public virtual  Item Item { get; set; }
    }
    public class Item
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public  virtual  ICollection<ElementOfOrder> ElementOfOrders { get; set; }
    }
}