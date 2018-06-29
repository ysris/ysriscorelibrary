using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class MacdTick
    {
        public DateTime DateTime { get; set; }
        public decimal MACD { get; set; }
        public decimal Signal { get; set; }
        public decimal Hist { get; set; }
    }
}
