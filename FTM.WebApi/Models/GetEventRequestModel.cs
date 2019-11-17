using System;
using System.Collections.Generic;

namespace FTM.WebApi.Models
{
    public class GetEventRequestModel
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public double Time { get; set; }
        public IEnumerable<string> CalendarIds { get; set; }
    }
}