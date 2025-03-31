using System;
using System.Collections;
using System.Collections.Generic;

namespace Bizza.Library
{
    public static class TimeUtil
    {
        public static DateTime DateTimeZero => new(1970, 1, 1, 0, 0, 0);
        public static long GetTimeStamp(this DateTime dateTime)
        {
            TimeSpan st = dateTime - DateTimeZero;
            return Convert.ToInt64(st.TotalMilliseconds);
        }
        public static long GetTimeStamp()
        {
            TimeSpan st = DateTime.Now - DateTimeZero;
            return Convert.ToInt64(st.TotalMilliseconds);
        }
        /*public static DateTime ToDateTime(this long stamp)
        {
            return new DateTime(DateTimeZero.Ticks + stamp * (long)Enum.ELTime.MS);
        }
        public static string FormatTime(long stamp)
        {
            DateTimeZero.ToString();
            long minute = stamp / (long)Enum.ETime.Minute;
            long second = (stamp - (minute * (long)Enum.ETime.Minute)) / (long)Enum.ETime.Second;
            string s = second < 10 ? "0" + second : second.ToString();
            return string.Concat(minute.ToString(), ":", s);
        }*/
    }
}
