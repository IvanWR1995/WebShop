using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.DAL;
using WebShop.Models;
using System.Web.Mvc;

namespace WebShop.Controllers
{
    [Authorize]
    public class OrderController : UserController
    {
        ApplicationDbContext context = new ApplicationDbContext();
        IUnitOfWork unit;
        public OrderController(IUnitOfWork unitIn)
        {
            unit = unitIn;
        }
        public OrderController()
        {
            unit = new UnitOfWork(context);
        }
        public ActionResult UserOrderView()
        {
            return View();

        }
        [Authorize(Roles = "Manager")]
        public ActionResult ManagerOrderView()
        {
            return View();

        }
        
        public JsonResult GetOrderWithItems()
        {
           
            Customer customer =  unit.GetCustomers.Get()
                .Where(c => c.Name == HttpContext.User.Identity.Name).FirstOrDefault();
            var OrdersAndItems = GetAllOrderWithItems(unit, customer.Id);
            return Json(new { Data = OrdersAndItems.ToArray() }, JsonRequestBehavior.AllowGet);
        }
       
        IEnumerable<dynamic> GetAllOrderWithItems(IUnitOfWork unit, Guid UserId, string UserName = null)
        {
            Func<dynamic, dynamic> deleg;
            if (String.IsNullOrEmpty(UserName))
            {
                deleg = o => new
                {
                    Items = unit.GetElementOfOrders.Get()
                      .Where(e => e.OrderId == o.Id).Select(s => new
                      {
                          Code = s.Item.Code,
                          Category = s.Item.Category,
                          Name = s.Item.Name,
                          Price = s.Item.Price,
                          Id = s.Item.Id,
                          Number = s.ItemCount
                      }).ToArray(),
                    OrderDate = o.OrderDate.ToShortDateString(),
                    OrderNumber = o.OrderNumber,
                    ShipmentDate = o.ShipmentDate == null? String.Empty : o.ShipmentDate.ToShortDateString(),
                    Status = o.Status,
                    Id = o.Id
                };
            }
            else
            {
                deleg = o => new
                {
                    UserName = UserName,
                    Items = unit.GetElementOfOrders.Get()
                      .Where(e => e.OrderId == o.Id).Select(s => new
                      {
                          Code = s.Item.Code,
                          Category = s.Item.Category,
                          Name = s.Item.Name,
                          Price = s.Item.Price,
                          Id = s.Item.Id,
                          ItemCount = s.ItemCount
                      }).ToArray(),
                    OrderDate = o.OrderDate.ToShortDateString(),
                    OrderNumber = o.OrderNumber,
                    ShipmentDate = o.ShipmentDate==null?String.Empty: o.ShipmentDate.ToShortDateString(),
                    Status = o.Status,
                    Id = o.Id
                };
            }
            var OrdersAndItems = unit.GetOrders.Get()
                    .Where(o => o.CustomerId == UserId)
                    .Select(deleg);
            return OrdersAndItems.ToArray();
        }
        [Authorize(Roles = "Manager")]
        public JsonResult GetOrdersForManager()
        {
         
            var AllOrderWithItems = unit.GetCustomers.Get()
                .SelectMany(s=>GetAllOrderWithItems(unit, s.Id, s.Name).ToArray());

            return Json(new { Data = AllOrderWithItems.ToArray() }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult Close(string Id)
        {
            
            Guid GuidId = Guid.Parse(Id);
            Order order = unit.GetOrders.GetByID(GuidId);
            if (order.Status == StatusOrder.New)
                return Json(new { success = false, Msg = "Заказ должен быть сначала подтвержден!" }, JsonRequestBehavior.AllowGet);
            order.Status = StatusOrder.Complete;
            unit.Save();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult Confirm(string Id, DateTime Date)
        {
         
            Guid GuidId = Guid.Parse(Id);
            Order order = unit.GetOrders.GetByID(GuidId);
            if (order.Status == StatusOrder.Complete)
                return Json(new { success = false,Msg = "Данный заказ уже выполнен.Его нельзя подтвердить!"  }, JsonRequestBehavior.AllowGet);
            order.ShipmentDate = Date;
            order.Status = StatusOrder.Confirm;
            unit.Save();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
      
        [HttpPost]
    
        public JsonResult Create( string Address, Item[] Items, Int32[] Numbers)
        {
          
            Order order = new Order();
            order.ShipmentDate =null ;
            order.OrderDate = DateTime.Now;
            order.Status = StatusOrder.New;
            order.Id = Guid.NewGuid();
            Customer customer = unit.GetCustomers.Get()
                .Where(c => c.Name == HttpContext.User.Identity.Name).FirstOrDefault();
            order.CurrentCustomer = customer;
            order.CustomerId = customer.Id;
            customer.CollectionOrders = new List<Order>();
            order.ElementOfOrders = new List<ElementOfOrder>();
            customer.CollectionOrders.Add(order);
          
            for (int i = 0; i != Items.Length; i++)
            {
                ElementOfOrder element = new ElementOfOrder();
                Item item = unit.GetItems.GetByID(Items[i].Id); 

                element.Id = Guid.NewGuid();
                element.ItemCount = Numbers[i];
                element.ItemId = Items[i].Id;
                element.ItemPrice = Items[i].Price;
                
                element.Item = item;
                item.ElementOfOrders.Add(element);

                element.OrderId = order.Id;
                element.CurrentOrder = order;
                order.ElementOfOrders.Add(element);
                
                unit.GetItems.Update(item);
                unit.GetElementOfOrders.Insert(element);

            }
            unit.GetOrders.Insert(order);
            unit.GetCustomers.Update(customer);
            unit.Save();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            Guid Id = Guid.Parse(id);
          
            try
            {
                Order order = unit.GetOrders.Get().Where(o => o.Id == Id).FirstOrDefault();
                Delete(order, unit);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, Msg = ex.Message });
            }
            return Json(new { success = true });
        }
       
    }
}