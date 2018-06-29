using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class IchimokuTick : Tick
    {
        public decimal? TenkanSen { get; internal set; }
        public decimal? KijuSen { get; internal set; }
        public decimal? SenkouSpanA { get; internal set; }
        public decimal? SenkouSpanB { get; internal set; }
        public double? ChikouSpan { get; internal set; }
    }
}
