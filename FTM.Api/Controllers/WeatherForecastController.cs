using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.AspNetCore;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FTM.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [GoogleScopedAuthorize("https://www.googleapis.com/auth/calendar.events.readonly")]
        public async Task<IEnumerable<WeatherForecast>> Get([FromServices] IGoogleAuthProvider auth, [FromServices] ClientInfo clientInfo)
        {
            var cred = await auth.GetCredentialAsync();
            var a = new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            };
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
    [Route("signin-oidc")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpGet]
        public void Get(string returnUrl)
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            //return new object { properties, GoogleOpenIdConnectDefaults.AuthenticationScheme };
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
            
        }

    }
}
