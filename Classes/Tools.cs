using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Classes
{
    /// <summary>
    /// Tools methods placed here
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Generate a datelist as a YYYYMMDD format
        /// </summary>
        /// <param name="from">from date</param>
        /// <param name="to">to date</param>
        /// <returns>list of string of YYYYMMDD</returns>
        /// 
        public static List<DateTime> GetCalendar(DateTime from, DateTime to)
        {
            var returnList = new List<DateTime>();

            DateTime cursor = @from;
            while (cursor <= to)
            {
                returnList.Add(cursor);
                cursor = cursor.AddDays(1);
            }
            return returnList.Select(a => a.Date).ToList();
        }

        public static List<DateTime> GetCalendar(Tuple<DateTime, DateTime> datetime) => GetCalendar(datetime.Item1, datetime.Item2);

        /// <summary>
        /// return a x possibility list
        /// Ex: For a delta of 1 with min=1 : 11 12 21 22
        /// </summary>
        /// <param name="min"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static List<Tuple<int, int>> GetWindowList(int min, int delta)
        {
            var x = new List<Tuple<int, int>>();
            for (int i = min; i <= delta; i++)
                for (int j = min; j <= delta; j++)
                    x.Add(new Tuple<int, int>(i, j));
            return x;
        }

        public static IEnumerable<Tuple<DateTime, DateTime>> MinutesPeriodsList(Tuple<DateTime, DateTime> period, int stepInMinutes)
            => MinutesPeriodsList(period.Item1, period.Item2, stepInMinutes);

        public static IEnumerable<Tuple<DateTime, DateTime>> MinutesPeriodsList(DateTime fromDate, DateTime toDate, int stepInMinutes)
            =>
            Enumerable.Range(0, Convert.ToInt32(Math.Floor((toDate - fromDate).TotalMinutes / stepInMinutes)))
            .Select(a => new Tuple<DateTime, DateTime>(
                fromDate.AddMinutes(a * stepInMinutes),
                fromDate.AddMinutes(a * stepInMinutes + stepInMinutes).AddSeconds(-1))
            );

        public static IEnumerable<Tuple<DateTime, DateTime>> SecondsPeriodsList(Tuple<DateTime, DateTime> period, int stepInSeconds)
            => SecondsPeriodsList(period.Item1, period.Item2, stepInSeconds);

        public static IEnumerable<Tuple<DateTime, DateTime>> SecondsPeriodsList(DateTime fromDate, DateTime toDate, int stepInSeconds)
        {
            var max =
                Convert.ToInt32(
                    Math.Ceiling((toDate - fromDate).TotalSeconds + stepInSeconds)
                    / stepInSeconds
                );
            return
                 Enumerable.Range(0, max)
                    .Select(a => new Tuple<DateTime, DateTime>(
                        fromDate.AddSeconds(a * stepInSeconds),
                        fromDate.AddSeconds(a * stepInSeconds + stepInSeconds))
                    );
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(decimal? unixTimeStamp) => UnixTimeStampToDateTime((double)unixTimeStamp);

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp) => UnixTimeStampToDateTime((double)unixTimeStamp);
        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp) => UnixTimeStampToDateTime(Convert.ToDouble(unixTimeStamp));

        public static double DateTimeToUnixTimeStamp(DateTime d) => (d.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
}
