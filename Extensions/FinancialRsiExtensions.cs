//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TicTacTec.TA.Library;
//using ysriscorelibrary.Helpers;
//using YsrisCoreLibrary.Models.Financial;

//namespace YsrisCoreLibrary.Extensions
//{
//    public static class FinancialRsiExtensions
//    {
//        public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<TimeSerieItem> data, int period = 14)
//                  => GetRsi(data.Select(a => new Tuple<DateTime, decimal>(a.date, (decimal)a.close)), period);

//        public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<Tick> data, int period = 14)
//            => GetRsi(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)), period);

//        public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<Tuple<DateTime, decimal>> data, int period = 14)
//        {
//            double[]
//                inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
//                , output = new double[inputClose.Length]
//                ;
//            int outBegIdx, outNbElement;

//            TicTacTec.TA.Library.Core.Rsi(
//                0,
//                inputClose.Length - 1,
//                inputClose,
//                period,
//                out outBegIdx,
//                out outNbElement,
//                output
//            );

//            return
//                output
//                .Take(outNbElement)
//                .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
//        }

//        public static IEnumerable<RsiTick> GetStochRSI(this IEnumerable<Tick> data, int window = 14)
//        {
//            double[]
//                inputClose = data.Select(a => Convert.ToDouble(a.Close)).ToArray()
//                , outputfastk = new double[inputClose.Length]
//                , outputfastd = new double[inputClose.Length]
//                ;
//            int outBegIdx, outNbElement;

//            int fastKPeriod = 5, fastDPeriod = 3;

//            TicTacTec.TA.Library.Core.StochRsi(
//                0,
//                inputClose.Length - 1,
//                inputClose,
//                window,
//                fastKPeriod,
//                fastDPeriod,
//                Core.MAType.Sma,
//                out outBegIdx,
//                out outNbElement,
//                outputfastk,
//                outputfastd
//            );
//            var outputfastk2 = outputfastk.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
//            var outputfastd2 = outputfastd.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
//            var result =
//                from x in data
//                join y in outputfastk2 on x.DateTime equals y.Item1
//                join z in outputfastd2 on x.DateTime equals z.Item1
//                select new RsiTick
//                {
//                    DateTime = x.DateTime,
//                    K = y.Item2,
//                    D = z.Item2,

//                };
//            return result;
//        }
//    }
//}
