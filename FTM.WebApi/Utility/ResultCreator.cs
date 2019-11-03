using FTM.WebApi.Models;
using Google.Apis.Calendar.v3.Data;
using System;

namespace FTM.WebApi.Utility
{
    public static class Creator
    {
        public static GetEventResultModel CreateEventResult(Event @event)
        {
            return new GetEventResultModel()
            {
                CalendarId = @event.Organizer.Email,
                CalendarName = @event.Organizer.DisplayName,
                HtmlLink = @event.HtmlLink
            };
        }
    }
}
