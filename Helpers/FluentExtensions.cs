using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Extensions
{
    public static class FluentExtensions
    {
        /// <summary>
        /// Divide a set in x subsets of partitionSize
        /// </summary>
        /// <typeparam name="Y">element type</typeparam>
        /// <param name="set">set to partitionate</param>
        /// <param name="partitionSize">max size of each part</param>
        /// <returns></returns>
        public static List<Y[]> Partitionate<Y>(this IEnumerable<Y> set, int partitionSize) where Y : class
        {
            var setCount = set.Count();
            var max = Math.Ceiling(1f * setCount / partitionSize);

            var outList = new List<Y[]>();

            for (int i = 0; i < max; i++)
            {
                var buff = set.Skip(i * partitionSize).Take(partitionSize).ToArray();
                outList.Add(buff);
            }

            return outList;
        }

        /// <summary>
        /// Convert to observable
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="me">enumerable to convert</param>
        /// <returns></returns>


        public static string Nl2Br(this string obj) => obj.Replace("\r\n", "<br />");

        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType)
        {

            if (input != null && suffixToRemove != null
              && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }
    }
}
