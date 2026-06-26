using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace webbannongsan.Controllers
{
    public class PaymentController : Controller
    {

        public ActionResult CreatePaymentUrl()
        {
            string vnp_TmnCode = ConfigurationManager.AppSettings["VNPay:vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["VNPay:vnp_HashSecret"];
            string vnp_Url = ConfigurationManager.AppSettings["VNPay:vnp_Url"];
            string vnp_ReturnUrl = ConfigurationManager.AppSettings["VNPay:vnp_ReturnUrl"];

            // Các thông tin thanh toán
            string amount = "100000"; // Đơn vị VND
            string orderId = DateTime.Now.Ticks.ToString(); // Mã đơn hàng
            string locale = "vn"; // Ngôn ngữ
            string orderInfo = "Thanh toán đơn hàng " + orderId;
            string ipAddress = Request.UserHostAddress;

            // Thông tin URL thanh toán
            SortedList<string, string> vnPayData = new SortedList<string, string>();
            vnPayData.Add("vnp_Version", "2.1.0");
            vnPayData.Add("vnp_Command", "pay");
            vnPayData.Add("vnp_TmnCode", vnp_TmnCode);
            vnPayData.Add("vnp_Amount", (Convert.ToInt32(amount) * 100).ToString());
            vnPayData.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPayData.Add("vnp_CurrCode", "VND");
            vnPayData.Add("vnp_IpAddr", ipAddress);
            vnPayData.Add("vnp_Locale", locale);
            vnPayData.Add("vnp_OrderInfo", orderInfo);
            vnPayData.Add("vnp_OrderType", "other");
            vnPayData.Add("vnp_ReturnUrl", vnp_ReturnUrl);
            vnPayData.Add("vnp_TxnRef", orderId);

            // Tạo chuỗi query
            string queryString = string.Join("&", vnPayData.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));

            // Tạo mã bảo mật (checksum)
            string signData = queryString;
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
            queryString += $"&vnp_SecureHash={vnp_SecureHash}";

            string paymentUrl = $"{vnp_Url}?{queryString}";
            return Redirect(paymentUrl);
        }

        // Phương thức mã hóa HmacSHA512
        private string HmacSHA512(string key, string data)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public ActionResult ConfirmPaymentClient()
{
    string vnp_HashSecret = ConfigurationManager.AppSettings["VNPay:vnp_HashSecret"];
    var vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
    var vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
    var vnp_Params = new SortedList<string, string>();

    foreach (string s in Request.QueryString)
    {
        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
        {
            vnp_Params.Add(s, Request.QueryString[s]);
        }
    }

    // Xóa `vnp_SecureHash` để tính lại
    vnp_Params.Remove("vnp_SecureHash");

    // Tạo chuỗi để mã hóa và kiểm tra lại mã hash
    string signData = string.Join("&", vnp_Params.Select(x => $"{x.Key}={x.Value}"));
    string checkSum = HmacSHA512(vnp_HashSecret, signData);

    if (checkSum == vnp_SecureHash && vnp_ResponseCode == "00")
    {
        ViewBag.Message = "Thanh toán thành công!";
    }
    else
    {
        ViewBag.Message = "Thanh toán thất bại!";
    }

    return View();
}

    }
}