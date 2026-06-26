using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        DB_TadNongSanEntities DB=new DB_TadNongSanEntities();
        public ActionResult OrderProduct()
        {
            var Ac = (Account)Session["Account"];
            if (Ac == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<int> cartQuantity = new List<int>();
            var AcID = Ac.AccountID;
            var carts = DB.Carts.Where(i => i.AccountID == AcID).ToList();
            List<Product> products = new List<Product>();
            float sumPrice = 0;
            foreach (var cart in carts)
            {
                Product product = DB.Products.Where(i => i.ProductID == cart.ProductID).FirstOrDefault();
                products.Add(product);
                cartQuantity.Add(cart.Quantity);
                sumPrice += (int)product.Price * cart.Quantity;
            }
            ViewBag.sumPrice = sumPrice;
            ViewBag.CartQuantity = cartQuantity;
            return View(products);
        }

    }
}