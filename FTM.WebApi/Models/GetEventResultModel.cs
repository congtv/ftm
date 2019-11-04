using System;

namespace FTM.WebApi.Models
{
    public class GetEventResultModel
    {
        public string CalendarId { get; set; }
        public string CalendarName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string HtmlLink { get; set; }
    }
}