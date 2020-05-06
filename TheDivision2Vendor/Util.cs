using System;

namespace TheDivision2Vendor
{
    public static class Util
    {
        private static int FLUSH_HOUR = 16;
        private static string FLUSH_HOUR_MM = "04";

        public static int GetWeekOfYear(DateTime dt)
        {
            int firstWeekend = 7 - Convert.ToInt32(DateTime.Parse(DateTime.Today.Year + ".01.01").DayOfWeek);
            int currentDay = dt.DayOfYear;
            return Convert.ToInt32(Math.Ceiling((currentDay - firstWeekend) / 7.0)) + 1;
        }

        public static DateTime GetThisTuesday()
        {
            var gmt8 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            if (gmt8.DayOfWeek == DayOfWeek.Tuesday && gmt8.Hour < FLUSH_HOUR)
            {
                gmt8 = gmt8.AddDays(-1);
            }
            gmt8 = DateTime.Parse($"{gmt8.Year}.{gmt8.Month}.{gmt8.Day} PM {FLUSH_HOUR_MM}:00:00");
            while (gmt8.DayOfWeek != DayOfWeek.Tuesday)
            {
                gmt8 = gmt8.AddDays(-1);
            }
            return gmt8;
        }

        public static DateTime GetNextTuesday()
        {
            var gmt8 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            if (gmt8.DayOfWeek == DayOfWeek.Tuesday && gmt8.Hour >= FLUSH_HOUR)
            {
                gmt8 = gmt8.AddDays(1);
            }
            gmt8 = DateTime.Parse($"{gmt8.Year}.{gmt8.Month}.{gmt8.Day} PM {FLUSH_HOUR_MM}:00:00");
            while (gmt8.DayOfWeek != DayOfWeek.Tuesday)
            {
                gmt8 = gmt8.AddDays(1);
            }
            return gmt8;
        }

        public static DateTime GetNextCassie(out bool onOffNow)
        {
            // gmt8 周三下午4点开 周四下午4点关 周六凌晨0点开 周日凌晨0点关 周一早上8点开 周二早上8点关
            var gmt8 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            var dest = gmt8;
            onOffNow = false;
            switch (gmt8.DayOfWeek)
            {
                case DayOfWeek.Tuesday:
                    if (gmt8.Hour < 8)
                    {
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 08:00:00");
                        onOffNow = true;
                    }
                    else
                    {
                        dest = gmt8.AddDays(1);
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} PM 04:00:00");
                    }
                    break;
                case DayOfWeek.Wednesday:
                    if (gmt8.Hour < 16)
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} PM 04:00:00");
                    else
                    {
                        dest = gmt8.AddDays(1);
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} PM 04:00:00");
                        onOffNow = true;
                    }
                    break;
                case DayOfWeek.Thursday:
                    if (gmt8.Hour < 16)
                    {
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} PM 04:00:00");
                        onOffNow = true;
                    }
                    else
                    {
                        dest = gmt8.AddDays(2);
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 12:00:00");
                    }
                    break;
                case DayOfWeek.Friday:
                    dest = gmt8.AddDays(1);
                    dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 12:00:00");
                    break;
                case DayOfWeek.Saturday:
                    dest = gmt8.AddDays(1);
                    dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 12:00:00");
                    onOffNow = true;
                    break;
                case DayOfWeek.Sunday:
                    dest = gmt8.AddDays(1);
                    dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 08:00:00");
                    break;
                case DayOfWeek.Monday:
                    if (gmt8.Hour < 8)
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 08:00:00");
                    else
                    {
                        dest = gmt8.AddDays(1);
                        dest = DateTime.Parse($"{dest.Year}.{dest.Month}.{dest.Day} AM 08:00:00");
                        onOffNow = true;
                    }
                    break;
                default:
                    break;
            }
            return dest;
        }
    }
}
