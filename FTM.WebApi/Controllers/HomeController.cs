using FTM.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CalendarsController calendarsController;

        public HomeController(ILogger<HomeController> logger, CalendarsController calendarsController)
        {
            _logger = logger;
            this.calendarsController = calendarsController;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Setting()
        {
            var result = calendarsController.GetCalendars();
            if(result is OkObjectResult ok)
            {
                return View(ok.Value);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Setting([FromForm]IEnumerable<CalendarInfoDto> calendarInfoDtos)
        {
            return View();
        }

        public IActionResult Duplicate()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
