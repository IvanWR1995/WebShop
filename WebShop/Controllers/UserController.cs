using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.Models.DAL;


namespace WebShop.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        protected  ApplicationDbContext context = new ApplicationDbContext();
        protected IUnitOfWork unit;
        public UserController()
        {
            unit = new UnitOfWork(context);
        }
        public UserController(IUnitOfWork unitIn)
        {
            unit = unitIn;
        }
       
        public void Delete(Order order, IUnitOfWork unit)
        {


            Customer customer = unit.GetCustomers.Get()
               .Where(c => c.Id == order.CustomerId).FirstOrDefault();

            customer.CollectionOrders.Remove(order);
            order.CustomerId = Guid.Empty;
            order.CurrentCustomer = null;
            var ElementsOfOrders = unit.GetElementOfOrders
                .Get().Where(e => e.OrderId == order.Id).ToArray();
            foreach (var element in ElementsOfOrders)
            {
                element.Item.ElementOfOrders.Remove(element);
                element.ItemId = Guid.Empty;
                element.Item = null;
                element.OrderId = Guid.Empty;
                element.CurrentOrder = null;
                unit.GetElementOfOrders.DeleteById(element.Id);
            }
            order.ElementOfOrders.Clear();
            unit.GetOrders.DeleteById(order.Id);
            unit.Save();
        }
        public void CreateCustomer(string UserName, IUnitOfWork unit)
        {
           
            Customer customer = new Customer();
            customer.Id = Guid.NewGuid();
            customer.Name = UserName;
            Random rand = new Random((int)DateTime.Now.Ticks);
            int RandDig = rand.Next(1000, 9999);
            string Code = RandDig.ToString();
            customer.Code = String.Join("-", Code, DateTime.Now.Year.ToString());
            customer.User = context.Users.FirstOrDefault(c => c.UserName == UserName);
            customer.ApplicationUserId = customer.User.Id;
            customer.User.Customer = customer;
            unit.GetCustomers.Insert(customer);
                unit.Save();
        }
    }
}