using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FTM.WebApi.Utility;

namespace FTM.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly FtmDbContext context;

        public CalendarController(FtmDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetRooms()
        {
            try
            {
                var calendars = context.RoomInfos.ToArray();
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
                    var calendars = context.RoomInfos.ToArray();

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
