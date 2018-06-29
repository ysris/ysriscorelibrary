using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Helpers;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialMfiExtensions
    {
        public static IEnumerable<Tuple<DateTime, decimal>> GetMfi(this IEnumerable<Tick> data, int window = 14)
        {
            double[]
                inputHigh = data.Select(a => Convert.ToDouble(a.High)).ToArray(),
                inputLow = data.Select(a => Convert.ToDouble(a.Low)).ToArray(),
                inputClose = data.Select(a => Convert.ToDouble(a.Close)).ToArray(),
                inputVolume = data.Select(a => Convert.ToDouble(a.Volume)).ToArray()
                , output = new double[inputClose.Length];
            int outBegIdx, outNbElement;

            TicTacTec.TA.Library.Core.Mfi(
                0,
                inputClose.Length - 1,
                inputHigh,
                inputLow,
                inputClose,
                inputVolume,
                window,
                out outBegIdx,
                out outNbElement,
                output
            );

            return
                output
                .Take(outNbElement)
                .Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
        }
    }
}
