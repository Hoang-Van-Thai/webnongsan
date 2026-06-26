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
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Product(string priceRange, string category, int?[] discount)
        {
            
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

            // Tạo danh sách giá đã giảm
            List<decimal> discountedPrices = new List<decimal>();

            // Cập nhật giá đã giảm cho từng sản phẩm
            foreach (var product in products)
            {
                // Lấy giá trị giảm giá cho sản phẩm hiện tại
                var discountValue = GetDiscountForProduct(product.ProductID);
                // Tính giá đã giảm
                var discountedPrice = product.Price * (1 - discountValue);
                discountedPrices.Add(discountedPrice); // Thêm giá đã giảm vào danh sách
                //product.DiscountedPrice = discountedPrice; // Cập nhật giá đã giảm trong model
            }

            // Lưu giá đã giảm vào ViewBag
            ViewBag.DiscountedPrices = discountedPrices;

            // Trả về view với danh sách sản phẩm đã có giá đã giảm
            return View(products);
            //return View(products);
        }
        public decimal GetDiscountForProduct(int productId)
        {
            var product = DB.Products.FirstOrDefault(p => p.ProductID == productId); // Lấy sản phẩm
            if (product == null || product.CouponID == null) return 0; // Nếu không tìm thấy hoặc không có coupon

            var coupon = DB.Coupons.FirstOrDefault(c => c.CouponID == product.CouponID); // Lấy coupon dựa trên CouponID
            return coupon != null ? (decimal)coupon.Discount : 0; // Trả về discount
        }
        public ActionResult DetailProduct(int? ProductID)
        {
    
            List<Product> products = DB.Products.Where(i => i.ProductID == ProductID).ToList();
            Product product = products.First();
            List<Feedback> feedbacks=DB.Feedbacks.Where(i=>i.ProductID == ProductID).ToList();
            List<string> nameAccount = new List<string>();
            List<string> imageAccount = new List<string>();
            Account account = null;
            foreach (var item in feedbacks)
            {
                account = DB.Accounts.Find(item.AccountID);
                nameAccount.Add(account.FirstName+ " " + account.LastName);
                imageAccount.Add(account.Avatar);
            }
            
            ViewBag.feedbacks=feedbacks;
            ViewBag.nameAccount=nameAccount;
            ViewBag.imageAccount=imageAccount;
            return View(product); 
        }
        public ActionResult Feedback(int ProductID,string feedbackContent,int ratetingStar)
        {
            Account account = (Account)Session["Account"];
            if (account == null) {
                return RedirectToAction("Login", "Account");
            }
            Feedback feedbackCheck=DB.Feedbacks.FirstOrDefault(i=>i.ProductID==ProductID && i.AccountID==account.AccountID);
            if (feedbackCheck != null) {
                TempData["erorr"] = "Bạn đã đánh giá sản phẩm này rồi";
                return RedirectToAction("DetailProduct", new { ProductID = ProductID });
            }
            Feedback feedback = new Feedback();
            feedback.ProductID = ProductID;
            feedback.FeedbackContent = feedbackContent;
            feedback.AccountID = account.AccountID;
            feedback.PostingDate=DateTime.Now;
            feedback.RatingStar =(short) ratetingStar;
            feedback.Status = true;
            DB.Feedbacks.Add(feedback);
            DB.SaveChanges();

            return RedirectToAction("DetailProduct", new { ProductID = ProductID });
        }

    }
}