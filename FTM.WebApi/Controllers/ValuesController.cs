using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTM.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Authorize]
        [GoogleScopedAuthorize("https://www.googleapis.com/auth/calendar")]
        public async Task<IEnumerable<string>> Get([FromServices] IGoogleAuthProvider auth, [FromServices] ClientInfo clientInfo)
        //public async Task<IEnumerable<string>> Get()
        {
            //var cred = await auth.GetCredentialAsync();
            //var credential = new BaseClientService.Initializer
            //{
            //    HttpClientInitializer = cred
            //};
            //var service = new CalendarService(credential);
            //// Define parameters of request.
            //EventsResource.ListRequest request = service.Events.List("primary");
            //request.TimeMin = DateTime.Now;
            //request.ShowDeleted = false;
            //request.SingleEvents = true;
            //request.MaxResults = 10;
            //request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            //// List events.
            //Events events = request.Execute();
            //Console.WriteLine("Upcoming events:");
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "763053086185-an7kopev3msfad5bdv6cp89srovc1g7f.apps.googleusercontent.com",
                    ClientSecret = "YUVJEaLeyHgjnItpO2LH2LJe"
                },
                DataStore = new FileDataStore("C:\\token.json"), // match the one defined in OnAuthorizationCodeReceived method
            });
            string userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var tokenResponse = flow.LoadTokenAsync(userId, CancellationToken.None).Result;
            var userCredential = new UserCredential(flow, userId, tokenResponse);
            var service = new CalendarService(new BaseClientService.Initializer() { HttpClientInitializer = userCredential});
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    [Route("signin-google")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpGet]
        //[GoogleScopedAuthorize("https://www.googleapis.com/auth/calendar.events.readonly")]
        public void Get()
        {
            var context = HttpContext;
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

    }
}
