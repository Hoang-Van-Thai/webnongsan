using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Areas.Admin.Controllers
{
    public class ManagerOrderController : Controller
    {
        // GET: Admin/Order
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        public ActionResult ListOrder()
        {
            List<Order> orders= DB.Orders.ToList();
            
            return View(orders);
        }
    }
}