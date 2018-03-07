using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class ItemConfiguration : EntityTypeConfiguration<Item>, IEntityConfiguration
    {
        public ItemConfiguration()
        {
            HasKey(o => o.Id);
            Property(o => o.Code).IsRequired();
            Property(o => o.Category).HasMaxLength(30);
            HasMany(o => o.ElementOfOrders)
           .WithRequired(r => r.Item)
           .HasForeignKey(f => f.ItemId);

        }
        public  void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}