using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarsController : ControllerBase
    {
        private readonly FtmDbContext context;

        public CalendarsController(FtmDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetCalendars()
        {
            try
            {
                var calendars = context.FtmCalendarInfo.ToArray();
                if (!calendars.Any())
                    return NoContent();
                var result = calendars.Select(x => x.CreateResult());
                return Ok(result);
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCalendarsUsable([FromBody] IEnumerable<CalendarInfoDto> calendarInfoDto)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (calendarInfoDto.Count() == 0)
                        return BadRequest();
                    var calendars = context.FtmCalendarInfo.ToArray();

                    foreach (var item in calendarInfoDto)
                    {
                        var update = calendars.First(x => x.CalendarId == item.RoomId);
                        update.IsUseable = item.IsUseable;
                        //update.CalendarName = editedRoom.RoomName;
                        //update.Description = editedRoom.Description;
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    var result = context.FtmCalendarInfo.Where(x => x.IsUseable).Select(x => new CalendarInfoDto()
                    {
                        RoomId = x.CalendarId,
                        RoomName = x.CalendarName,
                        IsUseable = x.IsUseable,
                        Description = x.Description
                    });
                    return Ok(result);
                }
                catch
                {
                    return BadRequest();
                }
            }
        }
    }
}