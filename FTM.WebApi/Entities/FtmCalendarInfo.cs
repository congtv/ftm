using System.ComponentModel.DataAnnotations;

namespace FTM.WebApi.Entities
{
    public class FtmCalendarInfo
    {
        [Key]
        public string CalendarId { get; set; }

        public string CalendarName { get; set; }
        public string Description { get; set; }
        public bool IsUseable { get; set; }
    }
}