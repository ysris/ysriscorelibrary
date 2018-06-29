using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Helpers;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialMovingAverageExtensions
    {
        public static IEnumerable<Tuple<DateTime, decimal>> GetSma(this IEnumerable<TimeSerieItem> data, int window)
            => GetSma(data.Select(a => new Tuple<DateTime, decimal>(a.date, (decimal)a.close)), window);

        public static IEnumerable<Tuple<DateTime, decimal>> GetSma(this IEnumerable<Tuple<DateTime, decimal>> data, int window)
        {
            double[]
                inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
                , output = new double[inputClose.Length];
            int outBegIdx, outNbElement;

            TicTacTec.TA.Library.Core.Sma(
                0,
                inputClose.Length - 1,
                inputClose,
                window,
                out outBegIdx,
                out outNbElement,
                output
            );

            return
                output
                .Take(outNbElement)
                .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
        }

        public static IEnumerable<MacdTick> GetMacd(this IEnumerable<TimeSerieItem> data, int fast = 12, int slow = 26, int signal = 9)
            => GetMacd(data.Select(a => new Tuple<DateTime, decimal>(a.date, (decimal)a.close)), fast, slow, signal);

        public static IEnumerable<MacdTick> GetMacd(this IEnumerable<Tick> data, int fast = 12, int slow = 26, int signal = 9)
            => GetMacd(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)), fast, slow, signal);


        public static IEnumerable<MacdTick> GetMacd(this IEnumerable<Tuple<DateTime, decimal>> data, int fast = 12, int slow = 26, int signal = 9)
        {
            double[]
                inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
                , outMACD = new double[inputClose.Length]
                , outMACDSignal = new double[inputClose.Length]
                , outMACDHist = new double[inputClose.Length]
                ;
            int outBegIdx, outNbElement;

            TicTacTec.TA.Library.Core.Macd(
                0,
                inputClose.Length - 1,
                inputClose,
                fast,
                slow,
                signal,
                out outBegIdx,
                out outNbElement,
                outMACD, outMACDSignal, outMACDHist
            );

            var outMACD2 = outMACD.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
            var outMACDSignal2 = outMACDSignal.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
            var outMACDHist2 = outMACDHist.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
            var resultofmacd =
                from x in data
                join y in outMACD2 on x.Item1.Date equals y.Item1.Date
                join z in outMACDSignal2 on x.Item1 equals z.Item1
                join u in outMACDHist2 on x.Item1 equals u.Item1
                select new MacdTick
                {
                    DateTime = x.Item1,
                    MACD = y.Item2,
                    Signal = z.Item2,
                    Hist = u.Item2

                };


            return resultofmacd;
        }

        public static IEnumerable<Tuple<DateTime, decimal>> GetEma(this IEnumerable<Tuple<DateTime, decimal>> data, int window)
        {
            double[]
                inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
                , output = new double[inputClose.Length];
            int outBegIdx, outNbElement;

            TicTacTec.TA.Library.Core.Ema(
                0,
                inputClose.Length - 1,
                inputClose,
                window,
                out outBegIdx,
                out outNbElement,
                output
            );

            return
                output
                .Take(outNbElement)
                .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
        }
    }
}
