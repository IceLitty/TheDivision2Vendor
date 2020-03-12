using System;

namespace TheDivision2Vendor
{
    public static class Util
    {
        public static int GetWeekOfYear(DateTime dt)
        {
            int firstWeekend = 7 - Convert.ToInt32(DateTime.Parse(DateTime.Today.Year + ".01.01").DayOfWeek);
            int currentDay = dt.DayOfYear;
            return Convert.ToInt32(Math.Ceiling((currentDay - firstWeekend) / 7.0)) + 1;
        }

        public static DateTime GetThisTuesday()
        {
            var n = DateTime.Now;
            n = DateTime.Parse($"{n.Year}.{n.Month}.{n.Day} PM 03:00:00");
            while (n.DayOfWeek != DayOfWeek.Tuesday)
            {
                n = n.AddDays(-1);
            }
            return n;
        }

        public static DateTime GetNextTuesday()
        {
            var n = DateTime.Now;
            n = DateTime.Parse($"{n.Year}.{n.Month}.{n.Day} PM 03:00:00");
            while (n.DayOfWeek != DayOfWeek.Tuesday)
            {
                n = n.AddDays(1);
            }
            return n;
            //var w = Convert.ToInt32(DateTime.Now.DayOfWeek);
            //if (w == 2)
            //{
            //    var s = (DateTime.Now.Hour * 60 + DateTime.Now.Minute) * 60;
            //    if (s >= 28800)
            //    {
            //        w += 7;
            //    }
            //}
            //else if (w > 2)
            //    w += 7;
            //var b = DateTime.Now.AddDays(2 - w);
            //var c = DateTime.Parse($"{b.Year}.{b.Month}.{b.Day} PM 03:00:00");
            //return c;
        }
    }
}
