using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class PoloniexTicker
    {
        public string symbol { get; set; }
        public int id { get; set; }
        public decimal last { get; set; }
        public decimal lowestAsk { get; set; }
        public decimal highestBid { get; set; }
        public decimal percentChange { get; set; }
        public decimal baseVolume { get; set; }
        public decimal quoteVolume { get; set; }
        public string isFrozen { get; set; }
        public decimal high24hr { get; set; }
        public decimal low24hr { get; set; }

        public decimal typicalPrice => (last + high24hr + low24hr) / 3;


        public override string ToString()
        {
            return $"({symbol} -> {typicalPrice})";
        }

    }
}
