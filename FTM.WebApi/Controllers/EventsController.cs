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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IConfiguration configuration;

        public EventsController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore, IConfiguration configuration)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] GetEventRequestModel requestModel)
        {
            if (requestModel.StartDateTime < requestModel.EndDateTime)
                return BadRequest("End date time must be greater than start date time");
            try
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                var result = new List<GetEventResultModel>();

                if (requestModel.CalendarIds.Any())
                {
                    var requestCalendars = context.RoomInfos.Where(x => requestModel.CalendarIds.Contains(x.RoomId)).ToArray();
                    var resultItems = await GetFreeTimes(service, requestCalendars, requestModel);
                    result.AddRange(resultItems);
                }
                else
                {
                    var usableCalendars = context.RoomInfos.Where(x => x.IsUseable).ToArray();
                    var resultItems = await GetFreeTimes(service, usableCalendars, requestModel);
                    result.AddRange(resultItems);
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all freetime in all calendar
        /// </summary>
        /// <param name="service"></param>
        /// <param name="calendars"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private async Task<IEnumerable<GetEventResultModel>> GetFreeTimes(CalendarService service, FtmRoomInfo[] calendars, GetEventRequestModel requestModel)
        {
            requestModel.StartDateTime = DateTime.Now.AddDays(1).CreateTime(new TimeSpan(0, 0, 0));
            requestModel.EndDateTime = DateTime.Now.AddDays(100);
            var resultDic = GetDateRangeRequest<List<GetEventResultModel>>(requestModel);

            #region Get time work in config
            var startTimeInDayConfig = configuration["Settings:StartTimeInDay"];
            var endTimeInDayConfig = configuration["Settings:EndTimeInDay"];
            TimeSpan startTimeInDay, endTimeInDay;
            startTimeInDay = TimeSpan.TryParse(startTimeInDayConfig, out startTimeInDay) ? startTimeInDay : new TimeSpan(7, 0, 0);
            endTimeInDay = TimeSpan.TryParse(endTimeInDayConfig, out endTimeInDay) ? endTimeInDay : new TimeSpan(20, 0, 0);
            #endregion

            foreach (var calendar in calendars)
            {
                EventsResource.ListRequest request = service.Events.List(calendar.RoomId);
                request.TimeMin = requestModel.StartDateTime;
                request.TimeMax = requestModel.EndDateTime;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 999;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events events = await request.ExecuteAsync();
              
                var itemDic = events.Items.GroupBy(x => x.Start.DateTime.Value.Date, x => x).ToDictionary(x => x.Key, x => x.ToList());

                #region Check day hasn't any event
                var dateDic = GetDateRangeRequest<List<Event>>(requestModel);
                foreach(var key in itemDic.Keys)
                {
                    dateDic[key] = itemDic[key];
                }

                foreach(var dateKey in dateDic.Keys)
                {
                    if (dateDic[dateKey].Count == 0)
                    {
                        resultDic[dateKey].Add(new GetEventResultModel()
                        {
                            CalendarId = "exadata.info_e5o5kj5jmng7q9au4ga2fsp3a4@group.calendar.google.com",
                            //CalendarId = calendar,
                            CalendarName = events.Items.First().Organizer.DisplayName,
                            StartTime = dateKey.CreateTime(startTimeInDay),
                            EndTime = dateKey.CreateTime(endTimeInDay),
                        });
                    }
                }
                #endregion

                foreach(var key in itemDic.Keys)
                {
                    var listEventByDay = itemDic[key];

                    for (var index = 0; index < listEventByDay.Count(); index++)
                    {
                        if (CheckEvent(index, listEventByDay, startTimeInDay, endTimeInDay, out List<GetEventResultModel> result))
                        {
                            if (resultDic.TryGetValue(key, out List<GetEventResultModel> dicResult))
                            {
                                dicResult.AddRange(result);
                            }
                        }
                    }
                }
            }

            return resultDic.Values.SelectMany(x => x);
        }

        /// <summary>
        /// Generic funtion create dictionary, this key is date time in range of request model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private Dictionary<DateTime, T> GetDateRangeRequest<T>(GetEventRequestModel requestModel) where T: new()
        {
            Dictionary<DateTime, T> dateDic = new Dictionary<DateTime, T>();

            for(int year = requestModel.StartDateTime.Value.Year; year <= requestModel.EndDateTime.Value.Year; year ++)
            {
                for(int month = requestModel.StartDateTime.Value.Month; month <= requestModel.EndDateTime.Value.Month; month++)
                {
                    for(int day = requestModel.StartDateTime.Value.Day; day <= requestModel.EndDateTime.Value.Day; day++)
                    {
                        var currentDay = new DateTime(year, month, day, 0, 0, 0);
                        dateDic.Add(currentDay, new T());
                    }
                }
            }
            return dateDic;
        }

        /// <summary>
        /// Check event then return list freetime.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="groupArray"></param>
        /// <param name="startTimeInDay"></param>
        /// <param name="endTimeInDay"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private bool CheckEvent(int index,List<Event> groupArray, TimeSpan startTimeInDay, TimeSpan endTimeInDay, out List<GetEventResultModel> results)
        {
            results = new List<GetEventResultModel>();

            var currentEvent = groupArray[index];
            var result = Creator.CreateEventResult(currentEvent);

            var startCurrentEvent = currentEvent.Start.DateTime.Value;
            var endCurrentEvent = currentEvent.End.DateTime.Value;

            if (startCurrentEvent.IsTimeNotBetween(startTimeInDay, endTimeInDay) ||
                endCurrentEvent.IsTimeNotBetween(startTimeInDay, endTimeInDay))
                return false;

            else if(groupArray.First() == currentEvent)
            {
                if (startCurrentEvent.IsTimeEquals(startTimeInDay))
                    return false;

                result.StartTime = startCurrentEvent.CreateTime(startTimeInDay);
                result.EndTime = startCurrentEvent;
                results.Add(result);

                if(groupArray.Count() == 1)
                {
                    var lastItem = Creator.CreateEventResult(currentEvent);
                    lastItem.StartTime = endCurrentEvent;
                    lastItem.EndTime = endCurrentEvent.CreateTime(endTimeInDay);
                    results.Add(lastItem);
                }

                if (GetFreeTimeByNextEvent(currentIndex: 0, groupArray, out GetEventResultModel nextItem))
                {
                    results.Add(nextItem);
                }
            }
            else if(groupArray.Last() == currentEvent)
            {
                if (endCurrentEvent.IsTimeEquals(endTimeInDay))
                    return false;

                else if (endCurrentEvent.IsTimeLessThan(endTimeInDay))
                {
                    result.StartTime = endCurrentEvent;
                    result.EndTime = endCurrentEvent.CreateTime(endTimeInDay);
                    results.Add(result);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if(GetFreeTimeByNextEvent(index, groupArray, out result))
                    results.Add(result);
            }
            return true;
        }

        /// <summary>
        ///  Get freetime by current event and next event. This function not for last event.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool GetFreeTimeByNextEvent(int currentIndex, List<Event> source, out GetEventResultModel result)
        {
            var currentEvent = source[currentIndex];
            result = Creator.CreateEventResult(currentEvent);

            if (source.Count == currentIndex + 1)
                return false;

            var nextEvent = source[currentIndex + 1];
            var startNextEvent = nextEvent.Start.DateTime.Value;

            if (currentEvent.End.DateTime.Value.IsTimeEquals(startNextEvent))
                return false;
            else if (currentEvent.End.DateTime.Value.IsTimeLessThan(startNextEvent))
            {
                result.StartTime = currentEvent.End.DateTime.Value;
                result.EndTime = startNextEvent;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
