using System;
using System.Threading;

namespace TheDivision2Vendor
{
    public static class MainFunc
    {
        private static Timer timer;
        public static TimeSpan Span = TimeSpan.Zero;
        public static DateTime DateTemp = DateTime.Now;

        public static void Init()
        {
            DateTemp = Util.GetNextTuesday();
            Span = DateTemp - DateTime.Now;
            timer = new Timer(state =>
            {
                if (Span.TotalSeconds > 1) Span = DateTemp - DateTime.Now;
                else
                {
                    DateTemp = Util.GetNextTuesday();
                    Span = DateTemp - DateTime.Now;
                }
                Logger.Put(LogPopType.Title, LogType.Info, "距离下次商人更新还差" + Span.ToString(@"dd\d\:hh\h\:mm\m\:ss\s"));
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }
}
