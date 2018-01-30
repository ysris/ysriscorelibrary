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
        /// Check if a date is in a given date range
        /// </summary>
        /// <param name="date">to check</param>
        /// <param name="rangeFrom">date range from</param>
        /// <param name="rangeTo">date range to</param>
        /// <returns></returns>
        public static bool Between(this DateTime date, DateTime rangeFrom, DateTime rangeTo) => date >= rangeFrom && date <= rangeTo;

        /// <summary>
        /// Check if a date is in a given date range
        /// </summary>
        /// <param name="date">to check</param>
        /// <param name="range">date range</param>
        /// <returns></returns>
        public static bool Between(this DateTime date, Tuple<DateTime, DateTime> range) => date >= range.Item1 && date <= range.Item2;

        public static bool Between(this DateTime date, (DateTime, DateTime) range) => Between(date, range.Item1, range.Item2);

        /// <summary>
        /// Foreach implementation for enumerable
        /// Why did i do that ? I don't know
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
                action(item);
        }

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
