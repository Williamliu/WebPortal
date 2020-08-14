using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Common
{
    public static class DateTimeHelper
    {
        public static long UTCSeconds(this System.DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
        public static DateTime FirstDayOfMonth(this System.DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        public static DateTime LastDayOfMonth(this System.DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month+1, 1).AddDays(-1);
        }

        public static DateTime IntDate(this long ticks)
        {
            if(ticks>0)
            {
                DateTime utcDT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return utcDT.AddSeconds(ticks);
            } else
            {
                return DateTime.MinValue;
            }

        }

        public static string YMDHM(this System.DateTime dt)
        {
            if (dt == DateTime.MinValue) return string.Empty;
            return dt.ToString("yyyy-MM-dd HH:mm");
        }
        public static string YMDHMS(this System.DateTime dt)
        {
            if (dt == DateTime.MinValue) return string.Empty;
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string YMD(this System.DateTime dt)
        {
            if (dt == DateTime.MinValue) return string.Empty;
            return dt.ToString("yyyy-MM-dd");
        }
        public static string HMS(this System.DateTime dt)
        {
            if (dt == DateTime.MinValue) return string.Empty;
            return dt.ToString("HH:mm:ss");
        }
        public static string HMS(this TimeSpan ts)
        {
            if (ts == TimeSpan.MinValue) return string.Empty;
            return ts.ToString(@"hh\:mm\:ss");
        }
        public static string HM(this System.DateTime dt)
        {
            if (dt == DateTime.MinValue) return string.Empty;
            return dt.ToString("HH:mm");
        }
        public static string HM(this TimeSpan ts)
        {
            if (ts == TimeSpan.MinValue) return string.Empty;
            return ts.ToString(@"hh\:mm");
        }
    }
}
