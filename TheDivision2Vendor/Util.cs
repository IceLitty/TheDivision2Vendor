﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TheDivision2Vendor
{
    public static class Util
    {
        private static int FLUSH_HOUR = 16;
        private static string FLUSH_HOUR_MM = "04";

        public static int GetWeekOfYear()
        {
            var dt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "China Standard Time");
            int firstWeekend = 7 - Convert.ToInt32(DateTime.Parse(DateTime.Today.Year + ".01.01").DayOfWeek);
            int currentDay = dt.DayOfYear;
            var sub = dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Monday || dt.DayOfWeek == DayOfWeek.Tuesday;
            return Convert.ToInt32(Math.Ceiling((currentDay - firstWeekend) / 7.0)) + 1 - (sub ? 1 : 0);
        }

        public static int GetWeekOfYear(DateTime dt)
        {
            int firstWeekend = 7 - Convert.ToInt32(DateTime.Parse(DateTime.Today.Year + ".01.01").DayOfWeek);
            int currentDay = dt.DayOfYear;
            return Convert.ToInt32(Math.Ceiling((currentDay - firstWeekend) / 7.0)) + 1;
        }

        [Obsolete("原先存放数据源内容调用本方法获取本周二日期，现采用获取数据源提供的日期，调用位置：TheDivision2Vendor/Config.cs#GetGearPath(String)", false)]
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

    public class RepeatItem <T>
    {
        public RepeatItem(T _obj)
        {
            Object = _obj;
            Counter = 1;
        }

        public T Object { get; set; }
        public int Counter { get; set; }

        public static List<RepeatItem<T>> GetRepeat(List<T> list)
        {
            var dict = new Dictionary<T, RepeatItem<T>>();
            foreach (var item in list)
            {
                if (dict.ContainsKey(item))
                {
                    dict[item].Counter++;
                }
                else
                {
                    dict.Add(item, new RepeatItem<T>(item));
                }
            }
            return dict.Values.ToList();
        }
    }
}
