using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using webbannongsan.Models;

namespace webbannongsan.Areas.Admin.Controllers
{
    
    public class ManagerAccountController : Controller
    {
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        // GET: Admin/ManagerAccount
        public ActionResult ListAccount()
        {
            List<Account> accounts = DB.Accounts.ToList();
            return View(accounts);
        }
        public ActionResult UpdateAccount(int AccountID)
        {
            Account account = DB.Accounts.FirstOrDefault(x => x.AccountID == AccountID);
            ViewBag.role=DB.Roles.ToList();
           
            return View(account);
        }
        [HttpPost]
        public ActionResult UpdateAccount(int AccountID, string FirstName,string LastName,string Email ,string PhoneNumber ,string RoleID)
        {
            Account account = DB.Accounts.FirstOrDefault(x => x.AccountID == AccountID);
            if(string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) )
            {
                ViewBag.error = "Vui lòng điền đúng thông tin";
                account.FirstName = FirstName;
                account.LastName = LastName;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;
                account.RoleID = RoleID;
                ViewBag.role = DB.Roles.ToList();
                return View("UpdateAccount", account);
            }
            if (IsValidPhoneNumber(PhoneNumber) == false)
            {
                ViewBag.error = "Số điên thoại không hợp lệ";
                account.FirstName = FirstName;
                account.LastName = LastName;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;
                account.RoleID = RoleID;
                ViewBag.role = DB.Roles.ToList();
                return View("UpdateAccount", account);
            }
            if ( IsValidEmail(Email) == false)
            {
                ViewBag.error = "Email không hợp lệ";
                account.FirstName = FirstName;
                account.LastName = LastName;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;
                account.RoleID = RoleID;
                ViewBag.role = DB.Roles.ToList();
                return View("UpdateAccount", account);
            }
            var email = DB.Accounts.FirstOrDefault(i=>i.Email==Email);
            if (Email != account.Email && email!=null)
            {
                ViewBag.error = "Email đã tồn tại";
                account.FirstName = FirstName;
                account.LastName = LastName;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;
                account.RoleID = RoleID;
                ViewBag.role = DB.Roles.ToList();
                return View("UpdateAccount", account);
            }
            var phoneNumber = DB.Accounts.FirstOrDefault(i=>i.PhoneNumber==PhoneNumber);
            if (PhoneNumber != account.PhoneNumber && phoneNumber!=null)
            {
                ViewBag.error = "Số điện thoại đã tồn tại";
                account.FirstName = FirstName;
                account.LastName = LastName;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;
                account.RoleID = RoleID;
                ViewBag.role = DB.Roles.ToList();
                return View("UpdateAccount", account);
            }
            account.FirstName = FirstName;
            account.LastName = LastName;
            account.Email = Email;
            account.PhoneNumber = PhoneNumber;
            account.RoleID = RoleID;
            DB.SaveChanges();

            return RedirectToAction("ListAccount" );
        }
        public ActionResult DeleteAccount(int AccountID)
        {
            Account account = DB.Accounts.FirstOrDefault(x => x.AccountID == AccountID);
            DB.Accounts.Remove(account);
            DB.SaveChanges();

            return RedirectToAction("ListAccount", account);
        }
        public ActionResult UpdateImage(int AccountID)
        {
            Account account = DB.Accounts.Find(AccountID);
            

            return View(account);
        }
        [HttpPost]
        public ActionResult UpdateImage(int AccountID, HttpPostedFileBase AccountImage)
        {
            //kiểm tra file có tồn tại không
            if (AccountImage == null)
            {
                return View(DB.Accounts.Find(AccountID));
            }
            if (AccountImage.ContentLength <= 0)
            {

                return View(DB.Accounts.Find(AccountID));
            }
            //tạo đường dẫn tương đối
            string path = "/image/Account";
            //đặt tên file
            string ext = System.IO.Path.GetExtension(AccountImage.FileName);
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
            AccountImage.SaveAs(savePath);
            // update link product save database
            var Account = DB.Accounts.Find(AccountID);
            Account.Avatar = path+ "/" + fileName;
            DB.SaveChanges();

            return RedirectToAction("ListAccount");

        }
        // Hàm kiểm tra số điện thoại
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Biểu thức chính quy kiểm tra số điện thoại (bắt đầu bằng 0 và có 10 chữ số)
            string pattern = @"^0\d{8}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        // Hàm kiểm tra địa chỉ email
        public static bool IsValidEmail(string email)
        {
            // Biểu thức chính quy kiểm tra địa chỉ email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
    }
}