using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>, IEntityConfiguration
    {
        public CustomerConfiguration()
        {
            HasKey(c => c.Id);
            Property(c => c.Name).IsRequired();
            Property(c => c.Code).IsRequired();
            Property(c => c.ApplicationUserId).IsRequired();
            HasMany(c => c.CollectionOrders)
            .WithRequired(s => s.CurrentCustomer)
            .HasForeignKey(f => f.CustomerId);
            HasRequired(o => o.User)
            .WithOptional(o => o.Customer);
            
        }
        public  void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add( this);
        }
    }
    
}