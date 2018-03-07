using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //CreateUser
            routes.MapRoute(
        "CreateUserAccount",
        "AccountController/CreateUser/{id}",
        new { controller = "Account", action = "CreateUser", id = UrlParameter.Optional });
            routes.MapRoute(
         "RemoveUserAccount",
         "AccountController/RemoveUser/{id}",
         new { controller = "Account", action = "RemoveUser", id = UrlParameter.Optional });
            routes.MapRoute(
         "UpdateDiscountAccount",
         "AccountController/UpdateDiscount/{id}",
         new { controller = "Account", action = "UpdateDiscount", id = UrlParameter.Optional });
          /*  routes.MapRoute(
          "UpdateRoleAccount",
          "AccountController/UpdateRole/{id}",
          new { controller = "Account", action = "UpdateRole", id = UrlParameter.Optional });*/
            routes.MapRoute(
           "UpdatePwdAccount",
           "AccountController/UpdatePwd/{id}",
           new { controller = "Account", action = " UpdatePwd", id = UrlParameter.Optional });
           
            routes.MapRoute(
           "UpdateLoginAccount",
           "AccountController/UpdateLogin/{id}",
           new { controller = "Account", action = "UpdateLogin", id = UrlParameter.Optional });
           
            routes.MapRoute(
          "CloseOrder",
          "OrderController/Close/{id}",
          new { controller = "Order", action = "Close", id = UrlParameter.Optional });
            routes.MapRoute(
           "ConfirmOrder",
           "OrderController/Confirm/{id}",
           new { controller = "Order", action = "Confirm", id = UrlParameter.Optional });
           
           routes.MapRoute(
           "GetAllOrders",
           "OrderController/GetOrdersForManager/{id}",
           new { controller = "Order", action = "GetOrdersForManager", id = UrlParameter.Optional });
           routes.MapRoute(
           "OrdersDelete",
           "OrderController/Delete/{id}",
           new { controller = "Order", action = "Delete", id = UrlParameter.Optional });
           routes.MapRoute(
           "UserOrders",
           "OrderController/GetOrderWithItems/{id}",
           new { controller = "Order", action = "GetOrderWithItems", id = UrlParameter.Optional });
            routes.MapRoute(
           "OrderCreate",
           "OrderController/Create/{id}",
           new { controller = "Order", action = "Create", id = UrlParameter.Optional });
            routes.MapRoute(
          "ItemWithoutOrderView",
          "ItemController/GetWithoutOrderView/{id}",
          new { controller = "Item", action = "GetWithoutOrderView", id = UrlParameter.Optional });
           /* routes.MapRoute(
           "ItemWithoutOrder",
           "ItemController/GetWithoutOrder/{id}",
           new { controller = "Item", action = "GetWithoutOrder", id = UrlParameter.Optional });*/
            routes.MapRoute(
            "ItemController",
            "ItemController/Get/{id}",
            new { controller = "Item", action = "Get", id = UrlParameter.Optional });

            routes.MapRoute(
           "ItemControllerDelete",
           "ItemController/Delete/{id}",
           new { controller = "Item", action = "Delete", id = UrlParameter.Optional }
           );
            routes.MapRoute(
            "ItemControllerCreate",
            "ItemController/Create/{id}",
            new { controller = "Item", action = "Create", id = UrlParameter.Optional }
            );
            routes.MapRoute(
            "ItemControllerUpdate",
            "ItemController/Update/{id}",
            new { controller = "Item", action = "Update", id = UrlParameter.Optional }
            );
        }
    }
}
