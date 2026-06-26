using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Areas.Admin.Controllers
{
    public class ManagerCategoryController : Controller
    {
        // GET: Admin/ManagerCategory
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        // GET: Admin/ManagerAccount
        public ActionResult ListCategory()
        {
            List<Category> category = DB.Categories.ToList();
            return View(category);
        }
        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCategory( string CategoryName)
        {   
            Category category1 = DB.Categories.FirstOrDefault(i=>i.Name== CategoryName);
            if (category1 != null)
            {
                ViewBag.error = "Danh mục đã tồn tại!";
                return View();
            }
            if (string.IsNullOrEmpty(CategoryName) == true)
            {
                ViewBag.error = "không hợp lệ vui lòng nhập lại!!";
                return View();
            }
            Category category = new Category();
            category.Name = CategoryName;
            DB.Categories.Add(category);
            DB.SaveChanges();
            // Thực hiện lưu dữ liệu hoặc hành động khác
            TempData["SuccessMessage"] = "Dữ liệu đã được lưu thành công!";
            return RedirectToAction("ListCategory");
        }
        public ActionResult UpdateCategory(int CategoryID)
        {
            Category category = DB.Categories.Find(CategoryID);
            return View(category);
        }
        [HttpPost]
        public ActionResult UpdateCategory(int CategoryID, string CategoryName)
        {
            Category category = DB.Categories.Find(CategoryID);
            category.Name = CategoryName;
            if (string.IsNullOrEmpty(CategoryName))
            {
                ViewBag.error = "Vui lòng điền tên danh mục";
                return View(category);
            }
            DB.SaveChanges();
            return RedirectToAction("ListCategory"); ;

        }

        public ActionResult DeleteCategory(int CategoryID)
        {
           Category category=DB.Categories.Find(CategoryID);
            DB.Categories.Remove(category);
            DB.SaveChanges();
            return RedirectToAction("ListCategory");
        }
        
    }
}