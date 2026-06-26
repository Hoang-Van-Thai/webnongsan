using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(string query)
        {
            DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
            List<Product> products = DB.Products.ToList();
            ViewBag.categories = DB.Categories.ToList();
            if (query != null) 
            { 
                products=products.Where(i=>i.ProductName.ToLower().Contains(query.ToLower())).ToList();
            }

            return View(products);
        }
    }
}