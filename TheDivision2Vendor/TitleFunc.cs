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
                    "距离下次商人更新还差【" + Span.ToString(@"dd\d\:hh\h\:mm\m\:ss\s") + "】" +
                    "    " + GetNextCassie()
                );
            }, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        public static string GetNextCassie()
        {
            bool onOffNow;
            var date = Util.GetNextCassie(out onOffNow);
            var span = date - TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            return onOffNow ? "隐藏商人已到来，离关闭还差【" + span.ToString(@"dd\d\:hh\h\:mm\m\:ss\s") + "】" : "隐藏商人关闭中，离开启还需【" + span.ToString(@"dd\d\:hh\h\:mm\m\:ss\s") + "】";
        }
    }
}
