using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.Models.DAL;

namespace WebShop.Controllers
{
    [Authorize]
    public class CustomerController : UserController
    {

        public CustomerController(IUnitOfWork unitIn)
            : base(unitIn)
        {
            
        }
        public CustomerController()
        {
            
        }
        [Authorize(Roles = "Manager")]
        public JsonResult UpdateDiscount(double Discount, string Id)
        {
           
            Customer customer = unit.GetCustomers.Get().Where(c=>c.ApplicationUserId == Id).FirstOrDefault();
            if(customer == null)
            {
                return Json(new { success = false, Msg = "Заказчик не найден!" });
            }
            customer.Discount = Discount;
            try
            {
                unit.Save();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, Msg = ex.Message });
            }
            
        }
       
    }
}