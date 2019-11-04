using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ClientInfo clientInfo;
        private readonly FtmDbContext context;
        private readonly FtmDataStore dataStore;

        public AuthenticateController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
        }

        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> Authenticate()
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                // Define parameters of request.
                var request = service.CalendarList.List();
                var result = await request.ExecuteAsync();

                var calendars = context.FtmCalendarInfo.ToArray();
                if (!calendars.Any())
                {
                    if (result.Items.Any())
                    {
                        foreach (var calendar in result.Items)
                        {
                            FtmCalendarInfo room = new FtmCalendarInfo()
                            {
                                CalendarId = calendar.Id,
                                CalendarName = calendar.Summary,
                                Description = calendar.Description,
                                IsUseable = false
                            };
                            context.FtmCalendarInfo.Add(room);
                        }
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (calendars.Length != result.Items.Count)
                    {
                        //Remove all room
                        context.FtmCalendarInfo.RemoveRange(calendars);

                        //Sync
                        foreach (var calendar in result.Items)
                        {
                            FtmCalendarInfo room = new FtmCalendarInfo()
                            {
                                CalendarId = calendar.Id,
                                CalendarName = calendar.Summary,
                                Description = calendar.Description,
                                IsUseable = false
                            };
                            context.FtmCalendarInfo.Add(room);
                        }
                        await context.SaveChangesAsync();
                    }
                }

                transaction.Commit();
                return Ok();
            }
        }
    }
}