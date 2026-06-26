using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Areas.Admin.Controllers
{
    public class ManagerCouponController : Controller
    {
        // GET: Admin/ManagerCoupon
        DB_TadNongSanEntities DB=new DB_TadNongSanEntities();
        public ActionResult ListCoupon()
        {
            
            return View(DB.Coupons.ToList());
        }
        public ActionResult CreateCoupon()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCoupon(string Name, float Discount,string Detail)
        {
            Coupon coupon = DB.Coupons.FirstOrDefault(i => i.Name == Name);
            if (coupon != null)
            {
                ViewBag.error = "Tên mã giảm giá đã tồn tại!";
                return View();
            }
            if (string.IsNullOrEmpty(Name) == true)
            {
                ViewBag.error = "Vui lòng nhập tên mã giảm giá!!";
                return View();
            }
            Coupon couponNew = new Coupon();
            couponNew.Name = Name;
            couponNew.Discount = Math.Round(Discount, 2);
            couponNew.PostingDate= DateTime.Now;
            couponNew.ExpiryDate= DateTime.Now.AddDays(2);
            couponNew.Status = true;
            Account account = (Account)Session["Account"];
            couponNew.AccountID=account.AccountID;
            couponNew.Detail = Detail;
            DB.Coupons.Add(couponNew);
            DB.SaveChanges();
            // Thực hiện lưu dữ liệu hoặc hành động khác
            TempData["SuccessMessage"] = "Dữ liệu đã được lưu thành công!";
            return RedirectToAction("ListCoupon");
        }
        public ActionResult UpdateCoupon(int CouponID)
        {
            Coupon coupon = DB.Coupons.Find(CouponID);
            ViewBag.discount=coupon.Discount;
            return View(coupon);
        }
        [HttpPost]
        public ActionResult UpdateCoupon(int CouponID,string Name, float Discount, string Detail)
        {
            Coupon coupon = DB.Coupons.Find(CouponID);
            coupon.Name = Name;
            coupon.Discount =Math.Round( Discount,2);
            coupon.Detail= Detail;
            if (string.IsNullOrEmpty(Name))
            {
                ViewBag.error = "Vui lòng điền tên mã giảm giá";
                return View(coupon);
            }
            DB.SaveChanges();
            return RedirectToAction("ListCoupon"); ;

        }

        public ActionResult DeleteCoupon(int CouponID)
        {
            Coupon coupon = DB.Coupons.Find(CouponID);
            DB.Coupons.Remove(coupon);
            DB.SaveChanges();
            return RedirectToAction("ListCoupon");
        }
    }
}