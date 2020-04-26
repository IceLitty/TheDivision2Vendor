using System;
using System.Threading;

namespace TheDivision2Vendor
{
    public static class TitleFunc
    {
        private static Timer timer;
        public static TimeSpan Span = TimeSpan.Zero;
        public static DateTime DateTemp;
        public static string updateStr = null;
        public static string pageStr = null;

        public static void Init()
        {
            DateTemp = Util.GetNextTuesday();
            Span = DateTemp - TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            timer = new Timer(state =>
            {
                if (Span.TotalSeconds > 1) Span = DateTemp - TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
                else
                {
                    DateTemp = Util.GetNextTuesday();
                    Span = DateTemp - TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
                }
                Logger.Put(LogPopType.Title, LogType.Info,
                    (string.IsNullOrWhiteSpace(updateStr) ? "" : updateStr + " ") +
                    (string.IsNullOrWhiteSpace(pageStr) ? "" : "[" + pageStr + "] ") +
                    "距离下次商人更新还差" + Span.ToString(@"dd\d\:hh\h\:mm\m\:ss\s")
                );
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }
}
