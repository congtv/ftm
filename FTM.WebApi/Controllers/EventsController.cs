using FTM.WebApi.Common;
using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ClientInfo clientInfo;
        private readonly FtmDataStore dataStore;
        private readonly FtmDbContext context;

        public EventsController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] GetEventRequestModel requestModel)
        {
            try
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                var result = new List<GetEventResultModel>();

                if (requestModel.CalendarIds.Any())
                {
                    var requestCalendars = context.RoomInfos.Where(x => requestModel.CalendarIds.Contains(x.RoomId)).ToArray();
                    var resultItems = await GetEventItems(service, requestCalendars, requestModel);
                    result.AddRange(resultItems);
                }
                else
                {
                    var usableCalendars = context.RoomInfos.Where(x => x.IsUseable).ToArray();
                    var resultItems = await GetEventItems(service, usableCalendars, requestModel);
                    result.AddRange(resultItems);
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        private async Task<IEnumerable<GetEventResultModel>> GetEventItems(CalendarService service, FtmRoomInfo[] calendars, GetEventRequestModel requestModel)
        {
            var result = new List<GetEventResultModel>();
            foreach (var calendar in calendars)
            {
                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List(calendar.RoomId);
                request.TimeMin = requestModel.StartDateTime;
                request.TimeMax = requestModel.EndDateTime;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 999;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                // List events.
                Events events = await request.ExecuteAsync();

                var items = events.Items;
                for (int index = 0; index < items.Count; index++)
                {
                    
                    if(events.Items.Last() == items[index])
                    {
                        //Last event
                    }


                }
            }
            return result;
        }
    }
}
