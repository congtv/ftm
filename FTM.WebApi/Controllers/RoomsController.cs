using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly FtmDbContext context;

        public RoomsController(FtmDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetRooms()
        {
            try
            {
                var rooms = context.RoomInfos.ToArray();
                if (!rooms.Any())
                    return NoContent();
                var result = rooms.Select(x => RoomInfoDto.Create(x));
                return Ok(result);
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateRoomsUsable([FromBody] IEnumerable<RoomInfoDto> roomInfoDtos)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var rooms = context.RoomInfos.ToArray();

                    foreach (var room in rooms)
                    {
                        var editedRoom = roomInfoDtos.First(x => x.RoomId == room.RoomId);

                        room.RoomName = editedRoom.RoomName;
                        room.IsUseable = editedRoom.IsUseable;
                        room.Description = editedRoom.Description;
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
