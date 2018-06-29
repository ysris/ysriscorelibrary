using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Extensions
{
    public static class EnumerableExtensions
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
    }
}
