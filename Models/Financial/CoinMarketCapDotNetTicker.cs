using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class CoinMarketCapDotNetTicker
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int rank { get; set; }
        public decimal price_usd { get; set; }
        public decimal price_btc { get; set; }
        // public decimal  24h_volume_usd  { get; set; }
        public decimal? market_cap_usd { get; set; }
        public decimal? available_supply { get; set; }
        public decimal? total_supply { get; set; }
        public decimal? percent_change_1h { get; set; }
        public decimal? percent_change_24h { get; set; }
        public decimal? percent_change_7d { get; set; }
        public string last_updated { get; set; }
    }
}
