using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialCctBboExtensions
    {
        public static Tuple<DateTime, double, double, double> CctBboSignalOfLastTick(this Dictionary<Tuple<DateTime, DateTime>, Tick> me)
        {
            var raw = me.Select(a => a.Value);
            var ts1 = me.GetBollingerBand().ToList();
            var ts2 = ts1.GetEma(30);

            // 1. Fist pass : CCTBO crossover
            var bfr =
                (
                    from x in raw
                    join y in ts1 on x.DateTime equals y.Item1
                    join z2 in ts2 on x.DateTime equals z2.Item1
                    let emacctbbo = z2.Item2
                    let tts1 = y.Item2
                    let tts2 = emacctbbo
                    select new
                    {
                        x.DateTime,
                        tts1,
                        tts2,
                        signal =
                            tts1 >= tts2
                                ? tts1 > 100
                                    ? 2 /*bullish cross and BUY emacctbbo*/
                                    : 1 /*bullish cross and HOLD emacctbbo*/
                                : tts1 < 0
                                    ? -1 /*bearish cross and SELL emacctbbo*/
                                    : 0 /*bearish cross and HOLD emacctbbo*/
                    }).ToList();

            // Smooth with SMA 5
            var secondSmoothSmaPeriod = 5;
            var smoothed = bfr.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.signal)).GetSma(secondSmoothSmaPeriod);

            // Get the signal 
            var qry =
            (
                from ts in raw
                join y in smoothed on ts.DateTime equals y.Item1
                select new
                {
                    ts.DateTime,
                    smoothedsignal = y.Item2,
                    smoothedsignalbinary =
                        y.Item2 >= 2
                            ? 2
                        : y.Item2 >= 0.8m
                            ? 1
                        : y.Item2 <= -1
                            ? -1
                        : 0
                }
            ).ToList();

            if (!qry.Any())
            {
                return null;
            }
            var lastEntity = qry.Last();
            return new Tuple<DateTime, double, double, double>(
                lastEntity.DateTime,
                Convert.ToDouble(bfr.Single(a => a.DateTime == lastEntity.DateTime).tts1),
                Convert.ToDouble(bfr.Single(a => a.DateTime == lastEntity.DateTime).tts2),
                Convert.ToDouble(qry.Single(a => a.DateTime == lastEntity.DateTime).smoothedsignalbinary)
            );
        }
    }
}
