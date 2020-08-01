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
        public static DateTime IntDate(this long ticks)
        {
            DateTime utcDT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return utcDT.AddSeconds(ticks);
        }

        public static string YMDHM(this System.DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm");
        }
        public static string YMDHMS(this System.DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string YMD(this System.DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static string HMS(this System.DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }
        public static string HMS(this TimeSpan ts)
        {
            return ts.ToString(@"hh\:mm\:ss");
        }
        public static string HM(this System.DateTime dt)
        {
            return dt.ToString("HH:mm");
        }
        public static string HM(this TimeSpan ts)
        {
            return ts.ToString(@"hh\:mm");
        }
    }
}
