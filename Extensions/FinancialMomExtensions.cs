//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TicTacTec.TA.Library;
//using YsrisCoreLibrary.Extensions;
//using YsrisCoreLibrary.Models;
//using YsrisCoreLibrary.Models.Financial;

//namespace ysriscorelibrary.Helpers
//{
//    public static class FinancialMomExtensions
//    {
      

//        public static IEnumerable<Tuple<Tick, double>> Mom(this IEnumerable<Tick> data, int windowSize)
//        {
//            var inputClose = data.Select(a => (float)a.Close).ToArray();
//            var output = new Double[inputClose.Length];
//            int outBegIdx, outNbElement;
//            TicTacTec.TA.Library.Core.Mom(startIdx: 0, endIdx: inputClose.Length - 1, inReal: inputClose, optInTimePeriod: windowSize, outBegIdx: out outBegIdx, outNBElement: out outNbElement, outReal: output);
//            return
//                output
//                .Take(outNbElement)
//                .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<Tick, double>(b, a));
//        }


       


//    }










  
//}