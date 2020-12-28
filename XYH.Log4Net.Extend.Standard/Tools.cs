using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYH.Log4Net.Extend.Standard
{
    public static class Tools
    {
        /// <summary>
        /// 获取系统当前时间
        /// </summary>
        /// <returns>系统当前时间</returns>
        public static DateTime GetSysDateTimeNow()
        {
            Instant now = SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// 获取一随机数(带上时间搓)
        /// </summary>
        /// <returns></returns>
        public static string GetDateRandomString()
        {
            return GetSysDateTimeNow().ToString("yyyyMMddHHmmssfff") + new Random().Next(1000, 9999);
        }
    }
}
