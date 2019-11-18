using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Cors;
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
        private readonly string baseLink;
        private TimeSpan startTimeInDay, endTimeInDay;

        public EventsController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore, IConfiguration configuration)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
            this.configuration = configuration;
            this.baseLink = this.configuration["Settings:BaseLink"];
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] GetEventRequestModel requestModel)
        {
            if (requestModel.StartDateTime > requestModel.EndDateTime)
                return BadRequest("End date time must be greater than start date time");
            try
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                var result = new List<GetEventResultModel>();

                if (requestModel.CalendarIds.Any())
                {
                    var requestCalendars = context.FtmCalendarInfo.Where(x => requestModel.CalendarIds.Contains(x.CalendarId)).ToArray();
                    var resultItems = await GetFreeTimes(service, requestCalendars, requestModel);
                    result.AddRange(resultItems);
                }
                else
                {
                    var usableCalendars = context.FtmCalendarInfo.Where(x => x.IsUseable).ToArray();
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
        private async Task<IEnumerable<GetEventResultModel>> GetFreeTimes(CalendarService service, FtmCalendarInfo[] calendars, GetEventRequestModel requestModel)
        {
            try
            {
                requestModel.StartDateTime = DateTime.Now.AddDays(1).CreateTime(new TimeSpan(0, 0, 0));
                requestModel.EndDateTime = DateTime.Now.AddDays(3);

                var resultDic = GetDateRangeRequest<List<GetEventResultModel>>(requestModel.StartDateTime.Value, requestModel.EndDateTime.Value);

                #region Get time work in config

                var startTimeInDayConfig = configuration["Settings:StartTimeInDay"];
                var endTimeInDayConfig = configuration["Settings:EndTimeInDay"];
                startTimeInDay = TimeSpan.TryParse(startTimeInDayConfig, out startTimeInDay) ? startTimeInDay : new TimeSpan(7, 0, 0);
                endTimeInDay = TimeSpan.TryParse(endTimeInDayConfig, out endTimeInDay) ? endTimeInDay : new TimeSpan(20, 0, 0);

                #endregion Get time work in config

                foreach (var calendar in calendars)
                {
                    EventsResource.ListRequest request = service.Events.List(calendar.CalendarId);
                    request.TimeMin = requestModel.StartDateTime;
                    request.TimeMax = requestModel.EndDateTime;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 999;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                    Events events = await request.ExecuteAsync();

                    var itemDic = events.Items.GroupBy(x => x.Start.DateTime.Value.Date, x => x).ToDictionary(x => x.Key, x => x.ToList());

                    #region Check day hasn't any event

                    var dateDic = GetDateRangeRequest<List<Event>>(requestModel.StartDateTime.Value, requestModel.EndDateTime.Value);
                    foreach (var key in itemDic.Keys)
                    {
                        dateDic[key] = itemDic[key];
                    }

                    foreach (var dateKey in dateDic.Keys)
                    {
                        if (dateDic[dateKey].Count == 0)
                        {
                            resultDic[dateKey].Add(new GetEventResultModel()
                            {
                                CalendarId = calendar.CalendarId,
                                CalendarName = itemDic.Any() ? events.Items.FirstOrDefault()?.Organizer?.DisplayName : calendar.CalendarName,
                                StartTime = dateKey.CreateTime(startTimeInDay),
                                EndTime = dateKey.CreateTime(endTimeInDay),
                                HtmlLink = $"{baseLink}{dateKey.CreateLinkParam()}"
                            });
                        }
                    }

                    #endregion Check day hasn't any event

                    foreach (var key in itemDic.Keys)
                    {
                        var listEventByDay = itemDic[key];

                        for (var index = 0; index < listEventByDay.Count(); index++)
                        {
                            if (CheckEvent(index, listEventByDay, out List<GetEventResultModel> result))
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
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Generic funtion create dictionary, this key is date time in range of request model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private Dictionary<DateTime, T> GetDateRangeRequest<T>(DateTime start, DateTime end) where T : new()
        {
            Dictionary<DateTime, T> dateDic = new Dictionary<DateTime, T>();
            var currentDay = start;
            while (currentDay.AddDays(1) <= end)
            {
                currentDay = currentDay.AddDays(1).CreateTime(new TimeSpan(0, 0, 0));
                dateDic.Add(currentDay, new T());
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
        private bool CheckEvent(int index, List<Event> groupArray, out List<GetEventResultModel> results)
        {
            results = new List<GetEventResultModel>();

            var currentEvent = groupArray[index];
            var result = currentEvent.CreateEventResult();

            var startCurrentEvent = currentEvent.Start.DateTime.Value;
            var endCurrentEvent = currentEvent.End.DateTime.Value;

            if (!startCurrentEvent.IsTimeBetween(startTimeInDay, endTimeInDay) ||
                !endCurrentEvent.IsTimeBetween(startTimeInDay, endTimeInDay))
                return false;
            else if (groupArray.First() == currentEvent)
            {
                if (startCurrentEvent.IsTimeEquals(startTimeInDay))
                    return false;

                result.StartTime = startCurrentEvent.CreateTime(startTimeInDay);
                result.EndTime = startCurrentEvent;
                result.HtmlLink = this.baseLink + startCurrentEvent.CreateLinkParam();
                results.Add(result);

                if (groupArray.Count() == 1)
                {
                    var lastItem = currentEvent.CreateEventResult();
                    lastItem.StartTime = endCurrentEvent;
                    lastItem.EndTime = endCurrentEvent.CreateTime(endTimeInDay);
                    lastItem.HtmlLink = this.baseLink + endCurrentEvent.CreateLinkParam();
                    results.Add(lastItem);
                }

                if (GetFreeTimeByNextEvent(0, groupArray, out GetEventResultModel nextItem))
                {
                    results.Add(nextItem);
                }
            }
            else if (groupArray.Last() == currentEvent)
            {
                if (endCurrentEvent.IsTimeEquals(endTimeInDay))
                    return false;
                else if (endCurrentEvent.IsTimeLessThan(endTimeInDay))
                {
                    result.StartTime = endCurrentEvent;
                    result.EndTime = endCurrentEvent.CreateTime(endTimeInDay);
                    result.HtmlLink = this.baseLink + endCurrentEvent.CreateLinkParam();
                    results.Add(result);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (GetFreeTimeByNextEvent(index, groupArray, out result))
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
            result = currentEvent.CreateEventResult();

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
                result.HtmlLink = this.baseLink + currentEvent.End.DateTime.Value.CreateLinkParam();
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet("duplicate")]
        public async Task<IActionResult> GetDuplicateEvents()
        {
            try
            {
                var startTimeInDayConfig = configuration["Settings:StartTimeInDay"];
                var endTimeInDayConfig = configuration["Settings:EndTimeInDay"];
                startTimeInDay = TimeSpan.TryParse(startTimeInDayConfig, out startTimeInDay) ? startTimeInDay : new TimeSpan(7, 0, 0);
                endTimeInDay = TimeSpan.TryParse(endTimeInDayConfig, out endTimeInDay) ? endTimeInDay : new TimeSpan(20, 0, 0);
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                var results = new List<EventErrorResult>();
                var usableCalendars = context.FtmCalendarInfo.Where(x => x.IsUseable).ToArray();
                var timeMin = DateTime.Now;
                var timeMax = int.TryParse(configuration["CheckViolateInDay"], out int day) ? DateTime.Now.AddDays(day) : DateTime.Now.AddDays(30);

                foreach (var calendar in usableCalendars)
                {
                    EventsResource.ListRequest request = service.Events.List(calendar.CalendarId);
                    request.TimeMin = timeMin;
                    request.TimeMax = timeMax;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 999;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                    Events events = await request.ExecuteAsync();

                    var itemDic = events.Items.GroupBy(x => x.Start.DateTime.Value.Date, x => x).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var key in itemDic.Keys)
                    {
                        var listEventByDay = itemDic[key];

                        for (var index = 0; index < listEventByDay.Count(); index++)
                        {
                            if (CheckViolateEvent(index, listEventByDay, out Event error))
                            {
                                results.Add(error.CreateErrorResult());
                            }
                        }
                    }
                }

                return Ok(results);
            }
            catch
            {
                return NotFound();
            }
        }

        private bool CheckViolateEvent(int index, List<Event> groupArray, out Event result)
        {
            result = new Event();
            var currentEvent = groupArray[index];
            var startCurrentEvent = currentEvent.Start.DateTime.Value;
            var endCurrentEvent = currentEvent.End.DateTime.Value;

            if (!startCurrentEvent.IsTimeBetween(this.startTimeInDay, this.endTimeInDay) ||
                !endCurrentEvent.IsTimeBetween(this.startTimeInDay, this.endTimeInDay) ||
                groupArray.Count < 1 ||
                groupArray.Last() == currentEvent)
                return false;
            else
            {
                var nextEvent = groupArray[index + 1];
                var startNextEvent = nextEvent.Start.DateTime.Value;
                if (startNextEvent < endCurrentEvent)
                {
                    result = nextEvent.Created > currentEvent.Created ? nextEvent : currentEvent;
                    return true;
                }
                else
                    return false;
            }
        }
    }
}