using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult GetRooms()
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
        [Authorize]
        public async Task<IActionResult> UpdateRoomsUsable([FromBody] IEnumerable<CalendarInfoDto> calendarInfoDto)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (calendarInfoDto.Count() == 0)
                        return BadRequest();
                    var calendars = context.FtmCalendarInfo.ToArray();

                    foreach (var calendar in calendars)
                    {
                        var editedRoom = calendarInfoDto.First(x => x.RoomId == calendar.CalendarId);

                        calendar.CalendarName = editedRoom.RoomName;
                        calendar.IsUseable = editedRoom.IsUseable;
                        calendar.Description = editedRoom.Description;
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return NoContent();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }
    }
}