using FTM.WebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Models
{
    public class CalendarInfoDto
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public string Description { get; set; }
        public bool IsUseable { get; set; }
    }
}
