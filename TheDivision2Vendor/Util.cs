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
            var gmt8 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            if (gmt8.DayOfWeek == DayOfWeek.Tuesday && gmt8.Hour < 15)
            {
                gmt8 = gmt8.AddDays(-1);
            }
            gmt8 = DateTime.Parse($"{gmt8.Year}.{gmt8.Month}.{gmt8.Day} PM 03:00:00");
            while (gmt8.DayOfWeek != DayOfWeek.Tuesday)
            {
                gmt8 = gmt8.AddDays(-1);
            }
            return gmt8;
        }

        public static DateTime GetNextTuesday()
        {
            var gmt8 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            if (gmt8.DayOfWeek == DayOfWeek.Tuesday && gmt8.Hour >= 15)
            {
                gmt8 = gmt8.AddDays(1);
            }
            gmt8 = DateTime.Parse($"{gmt8.Year}.{gmt8.Month}.{gmt8.Day} PM 03:00:00");
            while (gmt8.DayOfWeek != DayOfWeek.Tuesday)
            {
                gmt8 = gmt8.AddDays(1);
            }
            return gmt8;
        }
    }
}
