using System.Data.Entity;
using System.Security.Claims;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
 using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace WebShop.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public virtual Customer Customer { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
           
            
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ElementOfOrder> ElementsOfOrders { get; set; }
        public DbSet<Item> Items { get; set; }
        
        public ApplicationDbContext()
            : base("ShopContext", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new AppDbInitializer());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
         
            Type ourtype = typeof(IEntityConfiguration); // Базовый тип
            IEnumerable<Type> configurations = Assembly.GetAssembly(ourtype).GetTypes().Where(type => ourtype.IsAssignableFrom(type) && type != ourtype);  // using System.Linq
            foreach (Type configuration in configurations)
            {
                var InstanceConf = Activator.CreateInstance(configuration);
                configuration.GetMethod("AddConfiguration").Invoke(InstanceConf, new object[] { modelBuilder.Configurations }); // не уверен что будет работать
            }
            /*modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new OrderConfiguration());
            modelBuilder.Configurations.Add(new ItemConfiguration());
            modelBuilder.Configurations.Add(new ElementOfOrderConfiguration());
           */
            /*modelBuilder.Entity<ApplicationUser>().HasOptional(o => o.Customer)
            .WithRequired(o => o.User);*/
            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityRole ManagerRole = new IdentityRole { Name = "Manager" };
            IdentityRole UserRole = new IdentityRole { Name = "user" };
            roleManager.Create(ManagerRole);
            roleManager.Create(UserRole);
            var Manager = new ApplicationUser { Email = "Manager@mail.ru", UserName = "Manager@mail.ru" };
            IdentityResult result = userManager.Create(Manager, "Manager_123");
            if (result.Succeeded)
            {
                userManager.AddToRole(Manager.Id, ManagerRole.Name);
               
            }
            base.Seed(context);
        }
    }
}