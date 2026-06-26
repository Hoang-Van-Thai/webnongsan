using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbannongsan.Areas.Admin.Data
{
    public class DailyStatistic
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}