﻿using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public AuthenticateController(ClientInfo clientInfo, FtmDbContext context, FtmDataStore dataStore)
        {
            this.clientInfo = clientInfo;
            this.context = context;
            this.dataStore = dataStore;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authenticate()
        {
            using(var transaction = context.Database.BeginTransaction())
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(clientInfo, dataStore));
                // Define parameters of request.
                var request = service.CalendarList.List();
                var result = await request.ExecuteAsync();

                var roomControls = context.RoomInfos.ToArray();
                if (!roomControls.Any())
                {
                    if (result.Items.Any())
                    {
                        foreach (var calendar in result.Items)
                        {
                            FtmRoomInfo room = new FtmRoomInfo()
                            {
                                RoomId = calendar.Id,
                                RoomName = calendar.Summary,
                                Description = calendar.Description,
                                IsUseable = false
                            };
                            context.RoomInfos.Add(room);
                        }
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (roomControls.Length != result.Items.Count)
                    {
                        //Remove all room
                        context.RoomInfos.RemoveRange(roomControls);

                        //Sync
                        foreach (var calendar in result.Items)
                        {
                            FtmRoomInfo room = new FtmRoomInfo()
                            {
                                RoomId = calendar.Id,
                                RoomName = calendar.Summary,
                                Description = calendar.Description,
                                IsUseable = false
                            };
                            context.RoomInfos.Add(room);
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
