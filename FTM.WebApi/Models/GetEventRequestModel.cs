using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Models
{
    public class GetEventRequestModel
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public IEnumerable<string> CalendarIds { get; set; }
    }
}
