using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public interface IEntityConfiguration
    {
        void AddConfiguration(ConfigurationRegistrar registrar);
    }

  /*  public class CustomerConfiguration : EntityTypeConfiguration<Customer>, IEntityConfiguration
    {
        public static void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add( new CustomerConfiguration());
        }
    }*/
}
