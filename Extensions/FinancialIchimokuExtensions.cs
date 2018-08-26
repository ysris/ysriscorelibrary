//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ysriscorelibrary.Helpers;
//using YsrisCoreLibrary.Models.Financial;

//namespace YsrisCoreLibrary.Extensions
//{
//    public static class FinancialIchimokuExtensions
//    {
//        public static IEnumerable<IchimokuTick> GetIchimoku(this IEnumerable<Tick> data, int conversionLinePeriod = 9, int baseLinePeriod = 26, int laggingSpan2Period = 52, int displacement = 26)
//        {
//            var data2 = data.ToList();


//            var tenkanSen = data2.GetMovingWindow(conversionLinePeriod).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

//            var kijuSen = data2.GetMovingWindow(baseLinePeriod).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

//            var senkouSpanA =
//                    (from x in tenkanSen
//                     join y in kijuSen on x.Key equals y.Key into y2
//                     from y in y2.DefaultIfEmpty()
//                     select new { key = x.Key, value = (x.Value + y.Value) / 2 })
//                    .ToDictionary(a => a.key, a => a.value);

//            var senkouSpanB = data2.GetMovingWindow(laggingSpan2Period).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

//            var chikouSpan = new Dictionary<DateTime, double?>();
//            for (int i = 0; i < data2.Count() - displacement; i++)
//                chikouSpan.Add(data2[i].DateTime, Convert.ToDouble(data2[i + displacement].Close));
//            for (int i = data2.Count() - displacement; i < data2.Count(); i++)
//                chikouSpan.Add(data2[i].DateTime, double.NaN);

//            var qry =
//            (
//                from x in data.Skip(laggingSpan2Period)
//                select new IchimokuTick
//                {
//                    DateTime = x.DateTime,


//                    Open = x.Open,
//                    High = x.High,
//                    Low = x.Low,
//                    Close = x.Close,
//                    Volume = x.Volume,


//                    TenkanSen = tenkanSen.ContainsKey(x.DateTime) ? tenkanSen[x.DateTime] as Nullable<decimal> : null,
//                    KijuSen = kijuSen.ContainsKey(x.DateTime) ? kijuSen[x.DateTime] as Nullable<decimal> : null,
//                    SenkouSpanA = senkouSpanA.ContainsKey(x.DateTime) ? senkouSpanA[x.DateTime] as Nullable<decimal> : null,
//                    SenkouSpanB = senkouSpanB.ContainsKey(x.DateTime) ? senkouSpanB[x.DateTime] as Nullable<decimal> : null,
//                    ChikouSpan = chikouSpan.ContainsKey(x.DateTime) ? chikouSpan[x.DateTime] as Nullable<double> : null,
//                }
//            ).ToList();

//            return qry;
//        }
//    }
//}
