//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace YsrisCoreLibrary.Extensions
//{
//    public static class FinancialStDevExtensions
//    {
//        public static IEnumerable<Tuple<DateTime, decimal>> GetStDev(this IEnumerable<Tuple<DateTime, decimal>> data, int windowSize)
//        {
//            var inputClose = data.Select(a => (float)a.Item2).ToArray();
//            var output = new Double[inputClose.Length];
//            int outBegIdx, outNbElement;
//            TicTacTec.TA.Library.Core.StdDev
//            (
//                startIdx: 0,
//                endIdx: inputClose.Length - 1,
//                inReal: inputClose,
//                optInTimePeriod: windowSize,
//                optInNbDev: 1,
//                outBegIdx: out outBegIdx,
//                outNBElement: out outNbElement,
//                outReal: output
//            );
//            return output
//            .Take(outNbElement)
//            .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)))
//            ;
//        }
//    }
//}
