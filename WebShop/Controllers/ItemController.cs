using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.Models.DAL;

namespace WebShop.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        IUnitOfWork unit;
        public ItemController(IUnitOfWork unitIn)
        {
            unit = unitIn;
        }
        public ItemController()
        {
            unit = new UnitOfWork(context);
        }
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            

            return View("Index");
        }
       
        public JsonResult Get()
        {
          
            var Items = unit.GetItems.Get()
                .Select( i=>new {
                    Category=i.Category,
                    Code = i.Code,
                    Name =i.Name,
                    Price = i.Price,
                    Id = i.Id
                });
            return Json(new {Data=Items},JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetWithoutOrderView()
        {

            return View("GetWithoutOrderView");
        }
        
       /* public JsonResult GetWithoutOrder()
        {
         
            var Items = unit.GetItems.Get().Select(i => new { 
                Id=i.Id,
               Code=i.Code,
               Name=i.Name,
               Price = i.Price,
               Category = i.Category    }).ToArray();
           
            return Json(new { Data = Items }, JsonRequestBehavior.AllowGet);
        }*/
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult Delete(String Id)
        {
          
           
            
            try
            {
                unit.GetItems.DeleteById(Guid.Parse(Id));
                unit.Save();
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Data = Id, success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult Create(string Code, string Name, decimal Price, string Category)
        {
            Item item = new Item();
            item.Category = Category;
            item.Code = Code;
            item.Name = Name;
            item.Price = Price;
            item.Id = Guid.NewGuid();
            try
            {
                unit.GetItems.Insert(item);
                unit.Save();
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
            return  Json(new { Data = item ,success = true}, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult Update(Item item)
        {
           
            Item ItemOld = unit.GetItems.GetByID(item.Id);
            if (ItemOld != null)
            {
                ItemOld.Category = item.Category;
                ItemOld.Code = item.Code;
                ItemOld.Name = item.Name;
                ItemOld.Price = item.Price;
                unit.GetItems.Update(ItemOld);
               
            }
            try
            {
                unit.Save();
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Data = ItemOld, success = true }, JsonRequestBehavior.AllowGet);
        }
        
    }
}