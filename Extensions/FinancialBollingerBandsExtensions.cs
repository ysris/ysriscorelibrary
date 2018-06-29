using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Helpers;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialBollingerBandsExtensions
    {
        public static IEnumerable<Tuple<DateTime, decimal>> GetBollingerBand(this Dictionary<Tuple<DateTime, DateTime>, Tick> me)
        {
            var p = 65;

            var fuu = me.Select(a => a.Value).Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close));

            return
                from x in fuu
                join sma in fuu.GetSma(p).ToList() on x.Item1 equals sma.Item1
                join stdev in fuu.GetStDev(p).ToList() on x.Item1 equals stdev.Item1
                let standardDev = stdev.Item2 + 0.000000000001m
                let cctbbo = 100m * (x.Item2 + 2m * standardDev - sma.Item2) / (4m * standardDev)
                select new Tuple<DateTime, decimal>(x.Item1, Convert.ToDecimal(cctbbo));
        }
    }
}
