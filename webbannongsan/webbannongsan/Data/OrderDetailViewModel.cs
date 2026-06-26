using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbannongsan.Data
{
    public class OrderDetailViewModel
    {
        public DateTime OrderTime { get; set; }
        public int StatusOrder { get; set; }
        public string DefaultAddress { get; set; }
        public decimal OrderPrice { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public string Unit { get; set; }
        public int? CouponID { get; set; }
        public string CouponName { get; set; }
        public float? Discount { get; set; }
        public string CategoryName { get; set; }
    }
}