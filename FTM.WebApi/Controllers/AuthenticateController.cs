using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IConfiguration configuration;

        public AuthenticateController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore, IConfiguration configuration)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
            this.configuration = configuration;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Google")]
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
                        await Save(result.Items);
                    }
                }
                else
                {
                    var calendarDb = calendars.Select(x => x.CalendarId);
                    if (calendars.Length != result.Items.Count && result.Items.Any(x => !calendarDb.Contains(x.Id)))
                    {
                        //Remove all room
                        context.FtmCalendarInfo.RemoveRange(calendars);

                        await Save(result.Items);
                    }
                }

                transaction.Commit();
                return Ok();
            }
        }


        private async Task Save(IEnumerable<CalendarListEntry> calendars)
        {
            foreach (var calendar in calendars)
            {
                if (calendar.Id == configuration["Settings:AdminEmail"])
                    continue;
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
}