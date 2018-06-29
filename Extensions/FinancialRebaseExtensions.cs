using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Extensions
{
    public static class FinancialRebaseExtensions
    {
        public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<TimeSerieItem> data)
            => Rebase100(data.Select(a => new Tuple<DateTime, decimal>(a.date, (decimal)a.close)));

        public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<Tick> data)
            => Rebase100(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)));

        public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<Tuple<DateTime, decimal>> data)
        {
            var first = data.First().Item2;
            return
                from x in data
                select new Tuple<DateTime, decimal>(x.Item1, 1 + x.Item2 / first);
        }
    }
}
