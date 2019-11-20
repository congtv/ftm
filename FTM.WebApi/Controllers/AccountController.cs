using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using FTM.WebApi.Utility;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly FtmDbContext context;
        private readonly IConfiguration configuration;
        private readonly ClientInfo clientInfo;
        private readonly FtmDataStore dataStore;

        public AccountController(ILogger<AccountController> logger, FtmDbContext context, IConfiguration configuration, ClientInfo clientInfo, FtmDataStore dataStore)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
            this.clientInfo = clientInfo;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!IsAuthenticated(model.Username, model.Password))
            {
                model.IsLoginFail = true;
                model.Username = null;
                return View(model);
            }
               
            // create claims
            List<Claim> claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Email, model.Username)
                                };

            // create identity
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // create principal
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                    scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                    principal: principal,
                    properties: new AuthenticationProperties
                    {
                        //IsPersistent = true, // for 'remember me' feature
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
                    });

            return Redirect(model.ReturnUrl ?? "/");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string requestPath )
        {
            await HttpContext.SignOutAsync(
                    scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        private bool IsAuthenticated(string username, string password)
        {
#if DEBUG
            return (username == "admin" && password == "admin");
#else
            return (username == configuration["Settings:AdminEmail"] && password == configuration["Settings:Secret"]); 
#endif
        }

        [HttpGet]
        [Route("authenticate-google")]
        [Authorize(AuthenticationSchemes = "Google")]
        public async Task<IActionResult> Authenticate()
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                var service = new CalendarService(BaseClientServiceCreator.Create(context, clientInfo, dataStore));
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
            }
            return View();
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
                    IsUseable = true
                };
                context.FtmCalendarInfo.Add(room);
            }
            await context.SaveChangesAsync();
        }
    }
}
