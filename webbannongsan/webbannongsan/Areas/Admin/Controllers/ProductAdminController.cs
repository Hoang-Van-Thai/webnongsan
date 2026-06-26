using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using webbannongsan.Models;

namespace webbannongsan.Areas.Admin.Controllers
{
    public class ProductAdminController : Controller
    {
        // GET: Admin/Product
        DB_TadNongSanEntities DB =new DB_TadNongSanEntities();
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ListProduct(string priceRange, string category, int?[] discount)
        {
            
            List<Product> products = DB.Products.ToList();
            ViewBag.categories = DB.Categories.ToList();
            ViewBag.priceRange = priceRange;
            ViewBag.category = category;
            if (priceRange != "all" && priceRange != null)
            {
                string[] priceRanges = priceRange.Split('-');
                products = products.Where(i => i.Price >= int.Parse(priceRanges[0]) && i.Price <= int.Parse(priceRanges[1])).ToList();
            }
            if (category != "all" && category != null)
            {
                products = products.Where(i => i.Category.Name == category).ToList();
            }
            if (discount != null && discount.Length != 0)
            {
                foreach (var item in discount)
                {
                    products = products.Where(i => i.CouponID == item).ToList();
                }
            }
            return View(products);
        }
        public ActionResult CreateProduct()
        {
            ViewBag.categories = DB.Categories.ToList();
            Product product = new Product();
            product.PostingDate= DateTime.Now;
            product.Unit = "kg";
            return View(product);
        }
        [HttpPost]
        public ActionResult CreateProduct(Product product,string categoryname)
        {
            ViewBag.categories = DB.Categories.ToList();
            if (string.IsNullOrEmpty(product.ProductName) == true)
            {
                ViewBag.error = "không hợp lệ vui lòng nhập lại!!";
                return View(product);
            }
            product.AccountID = 39;
            Category categorygetid = DB.Categories.FirstOrDefault(i => i.Name == categoryname);
            product.CategoryID = categorygetid.CategoryID;
            DB.Products.Add(product);
            DB.SaveChanges();
            // Thực hiện lưu dữ liệu hoặc hành động khác
            TempData["SuccessMessage"] = "Dữ liệu đã được lưu thành công!";
            return RedirectToAction("ListProduct");
        }
        public ActionResult UpdateProduct(int id)
        {
            ViewBag.categories = DB.Categories.ToList();
            Product product = DB.Products.Find(id);
            var categoryProductID= DB.Categories.FirstOrDefault(i=>i.CategoryID==product.CategoryID);
            ViewBag.categoryName = categoryProductID.Name;
            ViewBag.coupons = DB.Coupons.ToList();
            Coupon couponTemp= DB.Coupons.FirstOrDefault(i=>i.CouponID==product.CouponID);
            if (couponTemp != null)
            {
                ViewBag.couponName = couponTemp.Name;
            }
            else
            {
                ViewBag.couponName ="0";
            }
            return View(product);
        }
        [HttpPost]
        public ActionResult UpdateProduct(Product productnew,string categoryname,int ?CouponID)
        {
            ViewBag.categories = DB.Categories.ToList();
            Product product = DB.Products.Find(productnew.ProductID);
            product.ProductName= productnew.ProductName;
            product.Price = productnew.Price;
            product.Image = productnew.Image;
            product.Detail = productnew.Detail;
            product.PostingDate= productnew.PostingDate;
            product.Unit= productnew.Unit;
            if (CouponID == 0)
            {
                product.CouponID = null;
            }
            product.CouponID =CouponID;
            var categoryTemp = DB.Categories.FirstOrDefault(i => i.Name == categoryname);
            product.CategoryID= categoryTemp.CategoryID;
            DB.SaveChanges();
            return RedirectToAction("ListProduct"); ;

        }

        public ActionResult DeleteProduct(int id)
        {
            var product = DB.Products.Find(id);
            DB.Products.Remove(product);
            DB.SaveChanges();
            return RedirectToAction("ListProduct");
        }
        public ActionResult UpdateImageProduct(int id)
        {
            Product product = DB.Products.Find(id);
            return View(product);
        
        }
        [HttpPost]
        public ActionResult UpdateImageProduct(int ProductID, HttpPostedFileBase ProductImage)
        {
            //kiểm tra file có tồn tại không
            if (ProductImage == null)
            {
                return View(DB.Products.Find(ProductID));
            }
            if (ProductImage.ContentLength <= 0)
            {

                return View(DB.Products.Find(ProductID));
            }
            //tạo đường dẫn tương đối
            string path = "/image/product";
            //đặt tên file
            string ext = System.IO.Path.GetExtension(ProductImage.FileName);
            string fileName = new Random().Next(1, 10000000).ToString() + ext;
            //xác định đường dẫn tuyệt đối
            string rootPath = Server.MapPath(path);
            if (System.IO.Directory.Exists(rootPath) == false)
            {
                System.IO.Directory.CreateDirectory(rootPath);
            }
            // xác định đường dẫn lưu =rootPath+fileName
            string savePath = rootPath + "/" + fileName;
            //save
            ProductImage.SaveAs(savePath);
            // update link product save database
            var product = DB.Products.Find(ProductID);
            product.Image =  fileName;
            DB.SaveChanges();

            return RedirectToAction("ListProduct");

        }
        private int GenerateNewAccountId()
        {
            // Lấy tất cả AccountId trong cơ sở dữ liệu
            var existingIds = DB.Accounts.Select(a => a.AccountID).ToList();

            // Tìm AccountId lớn nhất và cộng thêm 1
            int newId = existingIds.Any() ? existingIds.Max() + 1 : 1; // Bắt đầu từ 1 nếu không có id nào

            return newId;
        }
    }
}