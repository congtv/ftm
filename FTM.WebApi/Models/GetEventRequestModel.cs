using System;
using System.Collections.Generic;

namespace FTM.WebApi.Models
{
    public class GetEventRequestModel
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public double Time { get; set; }
        public IEnumerable<string> CalendarIds { get; set; }
    }
}