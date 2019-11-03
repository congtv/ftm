using System;

namespace FTM.WebApi.Utility
{
    public static class ExtensionMethod
    {
        public static bool IsDateEquals(this DateTime source, DateTime compare)
        {
            return source.Year == compare.Year && source.Month == compare.Month && source.Day == compare.Day;
        }

        public static bool IsDateGreaterThan(this DateTime source, DateTime compare)
        {
            return source.Year > compare.Year 
                || (source.Year == compare.Year 
                    && (source.Month > compare.Month || source.Day > compare.Day));
        }

        public static bool IsDateEqualsOrGreaterThan(this DateTime source, DateTime compare)
        {
            return source.Year >= compare.Year && source.Month >= compare.Month && source.Day >= compare.Day;
        }

        public static bool IsTimeEquals(this DateTime source, TimeSpan compare)
        {
            return source.Hour == compare.Hours && source.Minute == compare.Minutes && source.Second == compare.Seconds;
        }

        public static bool IsTimeEquals(this DateTime source, DateTime compare)
        {
            return source.Hour == compare.Hour && source.Minute == compare.Minute && source.Second == compare.Second;
        }

        public static bool IsTimeGreaterThan(this DateTime source, TimeSpan compare)
        {
            return source.Hour > compare.Hours
                || source.Hour == compare.Hours && source.Minute > compare.Minutes;
        }

        public static bool IsTimeGreaterThan(this DateTime source, DateTime compare)
        {
            return source.Hour > compare.Hour
                || source.Hour == compare.Hour && source.Minute > compare.Minute;
        }

        public static bool IsTimeEqualsOrGreaterThan(this DateTime source, TimeSpan compare)
        {
            return source.Hour >= compare.Hours
                || source.Hour >= compare.Hours && source.Minute >= compare.Minutes;
        }

        public static bool IsTimeLessThan(this DateTime source, DateTime compare)
        {
            return source.Hour < compare.Hour
                || source.Hour == compare.Hour && source.Minute < compare.Minute;
        }

        public static bool IsTimeLessThan(this DateTime source, TimeSpan compare)
        {
            return source.Hour < compare.Hours
                || source.Hour == compare.Hours && source.Minute < compare.Minutes;
        }

        public static bool IsTimeEqualsOrLessThan(this DateTime source, TimeSpan compare)
        {
            return source.Hour <= compare.Hours && source.Minute <= compare.Minutes;
        }

        public static bool IsTimeNotBetween(this DateTime source, TimeSpan start, TimeSpan end)
        {
            return (source.Hour < start.Hours || (source.Hour == start.Hours && source.Minute < start.Minutes) 
                    || source.Hour > end.Hours || (source.Hour == end.Hours && source.Minute > end.Minutes));
        }

        public static DateTime CreateTime(this DateTime source, TimeSpan time)
        {
            return new DateTime(source.Year, source.Month, source.Day, time.Hours, time.Minutes, time.Seconds);
        }
    }
}
