using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using webbannongsan.Models;

namespace webbannongsan.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Product(string priceRange, string category, int?[] discount)
        {
            DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
            List<Product> products = DB.Products.ToList();
            ViewBag.categories = DB.Categories.ToList();
            ViewBag.priceRange = priceRange;
            ViewBag.category = category;
            if (priceRange != "all" && priceRange!=null)
            {
                string[] priceRanges = priceRange.Split('-');
                products = products.Where(i => i.Price >= int.Parse(priceRanges[0]) && i.Price <= int.Parse(priceRanges[1])).ToList();
            }
            if (category != "all" && category != null)
            {
                products=products.Where(i=>i.Category.Name == category).ToList();
            }
            if (discount != null && discount.Length!=0  )
            {
                foreach (var item in discount)
                {
                    products = products.Where(i => i.CouponID == item).ToList();
                }
            }
            return View(products);
        }
        public ActionResult DetailProduct(int? detailPrID)
        {
            DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
            List<Product> products = DB.Products.Where(i => i.ProductID == detailPrID).ToList();
            Product product = products.First();
            return View(product);
            

            
        }

    }
}