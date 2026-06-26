using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using webbannongsan.Models;
using MailKit.Net.Smtp; // Sử dụng MailKit's SmtpClient
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Runtime.Remoting.Contexts;
using System.Data;
using webbannongsan.Data;
namespace webbannongsan.Controllers
{
    public class AccountController : Controller
    {
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {

            var user = DB.Accounts.FirstOrDefault(u => u.Email == email && u.Password == password);
            ViewBag.user = user;
            if (user != null)
            {
                Session["Account"] = user;
                if (user.RoleID == "ADMIN")
                {
                    return RedirectToAction("ListProduct", "ProductAdmin", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ";
                return View();


            }

        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Account model)
        {
            if (model == null)
            {
                return View(model);
            }
            if (string.IsNullOrEmpty(model.FirstName) == true)
            {
                return View(model);
            }
            if (string.IsNullOrEmpty(model.LastName) == true)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                return View(model);
            }
            else
            {
                var existingUser = DB.Accounts.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());
                if (existingUser != null)
                {
                    return View();
                }
            }
            if (string.IsNullOrEmpty(model.Password) == true)
            {
                return View(model);
            }
            if (string.IsNullOrEmpty(model.PhoneNumber) == true)
            {
                return View(model);
            }
            model.RoleID = "GUEST";
            DB.Accounts.Add(model);
            DB.SaveChanges();
            return RedirectToAction("Login");
        }
        public ActionResult UpdatePassword()
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            return View(account);
        }
        [HttpPost]
        public ActionResult UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            if (currentPassword != account.Password)
            {
                ViewBag.erorr = "Mật khẩu hiện tại không đúng!";
                return View(account);
            }
            if (string.IsNullOrEmpty(newPassword) == true)
            {
                ViewBag.erorr = "Vui lòng nhập mật khẩu mới!";
                return View(account);
            }
            if (string.IsNullOrEmpty(confirmPassword) == true)
            {
                ViewBag.erorr = "Vui lòng nhập xác nhận mật khẩu mới!";
                return View(account);
            }
            account.Password = confirmPassword;
            DB.SaveChanges();
            TempData["SuccessMessage"] = "Đã đổi mật khẩu thành công";
            return View(account);
        }
        public ActionResult Logout()
        {
            Session["Account"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Profile()
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            return View(account);
        }
        [HttpPost]
        public ActionResult Profile(Account model)
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            account.LastName = model.LastName;
            account.FirstName = model.FirstName;

            account.Email = model.Email;
            account.PhoneNumber = model.PhoneNumber;
            DB.SaveChanges();
            return RedirectToAction("Profile");
        }
        public ActionResult AddressProfile()
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            List<string> addressString = new List<string>();
            List<Address> addresses = DB.Addresses.Where(i => i.AccountID == Ac.AccountID).ToList();
            int defaultAdress = 0;
            if (Ac.DefaultAddress != null)
            {
                defaultAdress = (int)Ac.DefaultAddress;
                ViewBag.DefaultAdress = defaultAdress;
            }
            foreach (Address addressItem in addresses)
            {
                Ward ward = DB.Wards.FirstOrDefault(i => i.WardID == addressItem.WardID);
                District district = DB.Districts.FirstOrDefault(i => i.DistrictID == ward.DistrictID);
                Province province = DB.Provinces.FirstOrDefault(i => i.ProvinceID == district.ProvinceID);
                addressString.Add(addressItem.Name + " " + ward.Name + " " + district.Name + " " + province.Name);
            }
            ViewBag.addresses = addresses;

            ViewBag.addressString = addressString;
            return View(account);
        }
        public ActionResult DeleteAddress(int AddressID)
        {
            Address address = DB.Addresses.FirstOrDefault(i => i.AddressID == AddressID);
            if (address != null)
            {
                DB.Addresses.Remove(address);
                DB.SaveChanges();
            }
            return RedirectToAction("AddressProfile");
        }
        public ActionResult CreateAddress()
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            ViewBag.provinces = DB.Provinces.ToList();
            ViewBag.districts = DB.Districts.ToList();
            ViewBag.wards = DB.Wards.ToList();

            return View(account);
        }
        [HttpPost]
        public ActionResult CreateAddress(int provinceID, int districtID, int wardID, string Address)
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            if (string.IsNullOrEmpty(Address))
            {
                ViewBag.erorr = "Vui lòng nhập số nhà";
                return RedirectToAction("CreateAddress");
            }
            Address address = new Address();
            address.Name = Address.Trim();
            address.WardID = wardID;
            address.AccountID = account.AccountID;
            DB.Addresses.Add(address);
            DB.SaveChanges();

            return RedirectToAction("AddressProfile");
        }
        public ActionResult UpdateAddress(int AdressID)
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            ViewBag.provinces = DB.Provinces.ToList();
            ViewBag.districts = DB.Districts.ToList();
            ViewBag.wards = DB.Wards.ToList();

            Address address = DB.Addresses.Find(AdressID);
            ViewBag.addressID = address.AddressID;
            ViewBag.wardDef = DB.Wards.Find(address.WardID);
            ViewBag.districtDef = DB.Districts.Find(ViewBag.wardDef.DistrictID);
            ViewBag.provinceDef = DB.Provinces.Find(ViewBag.districtDef.ProvinceID);
            ViewBag.addressDef = address.Name;
            return View(account);
        }
        [HttpPost]
        public ActionResult UpdateAddress(int AdressID, int wardID, string Address)
        {
            Account Ac = (Account)Session["Account"];
            Account account = DB.Accounts.FirstOrDefault(i => i.AccountID == Ac.AccountID);
            Address address = DB.Addresses.Find(AdressID);
            if (string.IsNullOrEmpty(Address))
            {
                ViewBag.erorr = "Vui lòng nhập số nhà";
                return RedirectToAction("UpdateAddress");
            }
            address.Name = Address.Trim();
            address.WardID = wardID;
            address.AccountID = account.AccountID;
            DB.SaveChanges();
            return RedirectToAction("AddressProfile");
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var account = DB.Accounts.FirstOrDefault(i => i.Email == email);
            if (account != null)
            {
                var otp = new Random().Next(100000, 999999).ToString();
                Session["otp"] = otp;
                Session["email"] = email;
                // Gửi mã OTP qua email
                try
                {
                    SendOTPEmail(email, otp);
                    // Chuyển hướng đến trang xác nhận OTP
                    return RedirectToAction("ConfirmOTP");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi gửi email
                    ModelState.AddModelError("", "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau.");
                    // Bạn có thể ghi log lỗi ở đây
                }

            }
            return View();
        }
        // Trang xác nhận OTP
        public ActionResult ConfirmOTP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmOTP(string otp)
        {
            if(otp == (string)Session["otp"])
            {
                Session["otp"] = null;
                return RedirectToAction("ComfirmPassword");
            }
            ViewBag.erorr = "OTP không đúng";
            return View();
        }
        public ActionResult ComfirmPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ComfirmPassword(string newPassword,string ComfirmPassword)
        {
            if(newPassword!= ComfirmPassword)
            {
                ViewBag.erorr = "Mật khẩu không trùng khớp!";
                return View();
            }
            string email = (string)Session["email"];
            Account account = DB.Accounts.FirstOrDefault(i => i.Email == email);
            account.Password = ComfirmPassword;
            DB.SaveChanges();
            return RedirectToAction("Login");
        }
        
        public ActionResult OrderCustomer()
        {
            Account AC = (Account)Session["Account"];
            var result = (from o in DB.Orders
                          join od in DB.OrderDetails on o.OrderID equals od.OrderID
                          join p in DB.Products on od.ProductID equals p.ProductID
                          join c in DB.Coupons on od.Coupon equals c.CouponID into couponGroup
                          from c in couponGroup.DefaultIfEmpty() // Thực hiện LEFT JOIN
                          join cat in DB.Categories on p.CategoryID equals cat.CategoryID
                          where o.AccountID == AC.AccountID
                          select new OrderDetailViewModel
                          {
                              OrderTime = o.OrderTime,
                              StatusOrder = o.StatusOrder,
                              DefaultAddress = o.DefaultAddress,
                              OrderPrice = (decimal)o.Price,
                              ProductName = p.ProductName,
                              ProductPrice = p.Price,
                              Quantity = od.Quantity,
                              Image = p.Image,
                              Detail = p.Detail,
                              Unit = p.Unit,
                              CouponID = c.CouponID,
                              CouponName = c.Name,
                              Discount = (float?)c.Discount,
                              CategoryName = cat.Name
                          }).ToList();

            return View(result);
        }

        // Phương thức để tạo AccountId mới
        private int GenerateNewAccountId()
        {
            // Lấy tất cả AccountId trong cơ sở dữ liệu
            var existingIds = DB.Accounts.Select(a => a.AccountID).ToList();

            // Tìm AccountId lớn nhất và cộng thêm 1
            int newId = existingIds.Any() ? existingIds.Max() + 1 : 1; // Bắt đầu từ 1 nếu không có id nào

            return newId;
        }
        private void SendOTPEmail(string toEmail, string otp)
        {
            try
            {
                // Thông tin người gửi
                var fromAddress = new MailboxAddress("Vườn Nông Sản Tươi Ngon", "datbotp123@gmail.com");
                // Địa chỉ email nhận
                var toAddress = new MailboxAddress("", toEmail);

                // Lấy mật khẩu ứng dụng từ biến môi trường
                string fromPassword = "crgjrvmaykcrhdah";
                if (string.IsNullOrEmpty(fromPassword))
                {
                    Console.WriteLine("Lỗi: Mật khẩu ứng dụng không được thiết lập trong biến môi trường 'crgjrvmaykcrhdah'.");
                    return;
                }

                // Tiêu đề email
                string subject = "Mã OTP Đặt Lại Mật Khẩu";

                // Nội dung email với HTML
                string body = $"<p>Xin chào,</p><p>Mã OTP của bạn để đặt lại mật khẩu là: <strong>{otp}</strong></p><p>Mã OTP này có hiệu lực trong 15 phút.</p>";

                // Tạo đối tượng MimeMessage
                var message = new MimeMessage();
                message.From.Add(fromAddress);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };

                // Thiết lập SMTP và gửi email
                using (var smtp = new SmtpClient())
                {
                    // Kết nối đến máy chủ SMTP của Gmail trên port 587 với StartTLS
                    smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Xác thực với địa chỉ email và mật khẩu ứng dụng
                    smtp.Authenticate(fromAddress.Address, fromPassword);

                    // Gửi email
                    smtp.Send(message);
                    Console.WriteLine("Gửi email thành công.");

                    // Ngắt kết nối
                    smtp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có lỗi xảy ra
                Console.WriteLine("Gửi email không thành công: " + ex.Message);
            }
        }

    }
}