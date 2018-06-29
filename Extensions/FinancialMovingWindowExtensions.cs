using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialMovingWindowExtensions
    {
        /// <summary>
        /// Get a moving window of a time serie list
        /// </summary>
        /// <param name="data">data to treat</param>
        /// <param name="window">window size</param>
        /// <returns></returns>
        public static Dictionary<DateTime, IEnumerable<Tick>> GetMovingWindow(this IEnumerable<Tick> data, int window)
        {
            data = data.OrderBy(a => a.DateTime).ToList();

            var x = new Dictionary<DateTime, IEnumerable<Tick>>();
            for (int i = 0; i <= data.Count() - window; i++)
            {
                var outData = data.Skip(i).Take(window).ToList();
                x.Add(outData.Last().DateTime, outData);
            }

            return x;
        }
    }
}
