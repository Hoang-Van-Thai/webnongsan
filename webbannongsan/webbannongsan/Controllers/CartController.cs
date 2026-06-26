using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Models;

namespace webbannongsan.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        public ActionResult CartIndex()
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
            //foreach(var cart in carts)
            //{
            //    Product product = DB.Products.Where(i=>i.ProductID== cart.ProductID).FirstOrDefault();
            //    products.Add(product);
            //    cartQuantity.Add(cart.Quantity);
            //    sumPrice +=(int) product.Price* cart.Quantity;
            //}
            //ViewBag.sumPrice=sumPrice;
            //ViewBag.CartQuantity = cartQuantity;
            //return View(products);
            foreach (var cart in carts)
            {
                Product product = DB.Products.Where(i => i.ProductID == cart.ProductID).FirstOrDefault();
                if (product != null)
                {
                    products.Add(product);
                    cartQuantity.Add(cart.Quantity);

                    // Tính giá sau khi giảm giá
                    var discountValue = GetDiscountForProduct(product.ProductID);
                    var finalPrice = product.Price * (1 - discountValue);

                    sumPrice += (float)(finalPrice * cart.Quantity);
                }
            }

            ViewBag.sumPrice = sumPrice;
            ViewBag.CartQuantity = cartQuantity;
            return View(products);
        }
        public decimal GetDiscountForProduct(int productId)
        {
            var product = DB.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null || product.CouponID == null) return 0;

            var coupon = DB.Coupons.FirstOrDefault(c => c.CouponID == product.CouponID);
            return coupon != null ? (decimal)coupon.Discount : 0;
        }
        [HttpPost]
        public ActionResult CartIndex(int ProductID)
        {
            Cart newCart = new Cart();

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

            var cartTemp = DB.Carts.FirstOrDefault(i => i.ProductID == ProductID && i.AccountID == Ac.AccountID);
            if (cartTemp == null)
            {
                newCart.ProductID = ProductID;
                newCart.AccountID = AcID;
                newCart.Quantity = 1;
                DB.Carts.Add(newCart);
                DB.SaveChanges();
            }

            return RedirectToAction("CartIndex");
        }

        [HttpPost]
        public ActionResult UpdateCart(int[] selectedProducts, int[] quantity)
        {
            var Ac = (Account)Session["Account"];
            if (Ac == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var AcID = Ac.AccountID;
            var carts = DB.Carts.Where(i => i.AccountID == AcID).ToList();
            float sumPrice = 0;

            List<Coupon> Coupon = DB.Coupons.Where(i => i.AccountID == Ac.AccountID).ToList();
            ViewBag.Coupon = Coupon;


            // Danh sách sản phẩm đã chọn
            List<Product> selectedItems = new List<Product>();
            List<int> selectedQuantities = new List<int>();

            for (int i = 0; i < carts.Count; i++)
            {
                var cart = carts[i];
                if (selectedProducts != null && selectedProducts.Contains(cart.ProductID))
                {
                    Product product = DB.Products.FirstOrDefault(p => p.ProductID == cart.ProductID);
                    if (product != null)
                    {
                        var discountValue = GetDiscountForProduct(product.ProductID);
                        var finalPrice = product.Price * (1 - discountValue);
                        sumPrice += (float)(finalPrice * quantity[i]);

                        // Lưu sản phẩm và số lượng vào danh sách
                        selectedItems.Add(product);
                        selectedQuantities.Add(quantity[i]);
                    }
                }
            }

            var discounts = DB.Coupons.Select(c => c.Discount).ToList();
            // Lưu danh sách sản phẩm và số lượng vào TempData
            ViewBag.SelectedItems = selectedItems;
            ViewBag.SelectedQuantities = selectedQuantities;
            ViewBag.TotalPrice = sumPrice; // Cập nhật tổng giá
            ViewBag.SumPrice = sumPrice; // Lưu vào ViewBag để truyền sang View

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
            return View(); // Chuyển đến trang xác nhận đơn hàng

        }
        [HttpPost]

        public ActionResult ConfirmOrder(List<Product> selectedItems, List<int> selectedQuantities, string address, float totalPrice, float Discount)
        {
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var selectedItem = selectedItems[i];
                var selectQ = selectedQuantities[i];
                Product product = DB.Products
                    .FirstOrDefault(m => m.ProductID == selectedItem.ProductID);
                if (product != null && product.Quantity >= selectQ)
                {
                    product.Quantity = product.Quantity - selectedQuantities[i];
                    DB.SaveChanges();
                }
                else
                {
                    TempData["ErrorMessage"] = "Số lượng sản phẩm không đủ";
                    return RedirectToAction("CartIndex");
                }

            }
            var Ac = (Account)Session["Account"];
            Order newOrder = new Order
            {
                AccountID = Ac.AccountID,
                DefaultAddress = address,
                OrderTime = DateTime.Now,
                DeliveryTime = DateTime.Now.AddDays(3),
                StatusOrder = 2,
                Price = (decimal)(totalPrice * (1 - Discount)) // Sử dụng totalPrice.Value vì nó đã được kiểm tra  
            };

            DB.Orders.Add(newOrder);
            DB.SaveChanges();
           

            // Lấy OrderID vừa tạo để lưu OrderDetails
            var orderId = newOrder.OrderID;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                int item = selectedItems[i].AccountID;
                var coupon = DB.Coupons.FirstOrDefault(m => m.CouponID == item);
                var discountProduct = 1;
                if (coupon == null)
                {
                    discountProduct = 1;
                }
                else
                {
                    discountProduct = (int)coupon.Discount;
                }

                OrderDetail orderDetail = new OrderDetail
                {
                    OrderID = orderId,
                    ProductID = selectedItems[i].ProductID,
                    Quantity = selectedQuantities[i],
                    Coupon = discountProduct
                };
                DB.OrderDetails.Add(orderDetail);
                DB.SaveChanges();
            }
            
            ViewBag.total = totalPrice;
            ViewBag.address = address;
            return View();


        }

        public ActionResult RemoveFromCart(int productId)
        {
            Cart product = DB.Carts.FirstOrDefault(m => m.ProductID==productId);
            DB.Carts.Remove(product);
            DB.SaveChanges();
            return RedirectToAction("CartIndex");
        }



    }
}