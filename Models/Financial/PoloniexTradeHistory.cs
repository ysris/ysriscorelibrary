using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class PoloniexTradeHistory
    {
        public string globalTradeID { get; set; }
        public int tradeID { get; set; }
        public DateTime date { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public decimal total { get; set; }
        public decimal fee { get; set; }
        public string orderNumber { get; set; }
        public string type { get; set; }
        public string category { get; set; }
    }
}
