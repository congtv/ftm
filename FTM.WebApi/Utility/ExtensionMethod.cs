using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Google.Apis.Calendar.v3.Data;
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

        public static bool IsTimeBetween(this DateTime source, TimeSpan start, TimeSpan end)
        {
            var startDateTime = source.CreateTime(start);
            var endDateTime = source.CreateTime(end);
            return startDateTime <= source && source <= endDateTime;
        }

        public static DateTime CreateTime(this DateTime source, TimeSpan time)
        {
            return new DateTime(source.Year, source.Month, source.Day, time.Hours, time.Minutes, time.Seconds);
        }

        public static string CreateLinkParam(this DateTime source)
        {
            return $"{source.Year}/{source.Month}/{source.Day}";
        }

        public static EventErrorResult CreateErrorResult(this Event @event)
        {
            return new EventErrorResult()
            {
                Summary = @event.Summary,
                Creator = @event.Creator.Email,
                HtmlLink = @event.HtmlLink,
                Description = @event.Description
            };
        }

        public static GetEventResultModel CreateEventResult(this Event @event)
        {
            return new GetEventResultModel()
            {
                CalendarId = @event.Organizer.Email,
                CalendarName = @event.Organizer.DisplayName,
                HtmlLink = @event.HtmlLink
            };
        }

        public static CalendarInfoDto CreateResult(this FtmCalendarInfo calendar)
        {
            return new CalendarInfoDto()
            {
                RoomName = calendar.CalendarName,
                Description = calendar.Description,
                IsUseable = calendar.IsUseable,
                RoomId = calendar.CalendarId
            };
        }

        public static TimeSpan DoubleToTimeSpam(this double num)
        {
            var hour = (int)num;
            var minutes = (int)(num - hour) * 60;
            return new TimeSpan(hour, minutes, 0);
        }
    }
}