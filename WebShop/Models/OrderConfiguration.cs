using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class OrderConfiguration : EntityTypeConfiguration<Order>, IEntityConfiguration
    {
        public OrderConfiguration() 
        {
            ToTable("OrderTest");
            HasKey(o => o.Id);
            Property(o => o.CustomerId).IsRequired();
            Property(o => o.OrderDate).IsRequired();
            Property(o => o.OrderNumber).IsRequired();
            HasMany(o => o.ElementOfOrders)
            .WithRequired(r => r.CurrentOrder)
            .HasForeignKey(f => f.OrderId);
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}