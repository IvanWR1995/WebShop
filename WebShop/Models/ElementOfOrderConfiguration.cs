using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class ElementOfOrderConfiguration:EntityTypeConfiguration<ElementOfOrder>,IEntityConfiguration
    {
        public ElementOfOrderConfiguration()
        {
            HasKey(o => o.Id);
            Property(o => o.OrderId).IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("OrderIdItemId", 1) { IsUnique=true}));
            Property(o => o.ItemId).IsRequired()
                 .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("OrderIdItemId", 2) { IsUnique = true }));
            Property(o => o.ItemCount).IsRequired();
            Property(o => o.ItemPrice).IsRequired();
         
     
        }
        public  void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}