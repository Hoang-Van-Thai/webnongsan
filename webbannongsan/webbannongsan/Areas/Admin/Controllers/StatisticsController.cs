using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webbannongsan.Areas.Admin.Data;
using webbannongsan.Models;
namespace webbannongsan.Areas.Admin.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Admin/Statistics
        DB_TadNongSanEntities DB = new DB_TadNongSanEntities();
        public ActionResult OrderStatusStatistics()
        {
            // Đếm số lượng đơn hàng theo từng trạng thái
            var orderStatusCounts0 = DB.Orders.Where(i => i.StatusOrder == 0).ToList();
            var orderStatusCounts1 = DB.Orders.Where(i => i.StatusOrder == 1).ToList();

            var orderStatusCounts2 = DB.Orders.Where(i=>i.StatusOrder==2).ToList();

            // Chuyển dữ liệu qua ViewBag
            ViewBag.OrderStatusCounts0 = orderStatusCounts0;
            ViewBag.OrderStatusCounts1 = orderStatusCounts1;
            ViewBag.OrderStatusCounts2 = orderStatusCounts2;

            var total0= orderStatusCounts0.Sum(i=>i.Price);
            var total1= orderStatusCounts1.Sum(i=>i.Price);
            var total2= orderStatusCounts2.Sum(i=>i.Price);
            ViewBag.total0 = total0;
            ViewBag.total1 = total1;
            ViewBag.total2 = total2;

            var dailyStatistics = DB.Orders
            .GroupBy(o => System.Data.Entity.DbFunctions.TruncateTime(o.OrderTime)) // Nhóm theo ngày
            .Select(g => new DailyStatistic
            {
                Date = g.Key.Value, // Ngày
                TotalRevenue = g.Sum(o => o.Price) ?? 0, // Tổng doanh thu
                OrderCount = g.Count() // Số lượng đơn hàng
            })
            .ToList();

            ViewBag.DailyStatistics = dailyStatistics;


            return View();
        }

    }
}