using FTM.WebApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FTM.WebApi.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CalendarsController calendarsController;
        private readonly EventsController eventsController;

        public HomeController(ILogger<HomeController> logger, CalendarsController calendarsController, EventsController eventsController)
        {
            _logger = logger;
            this.calendarsController = calendarsController;
            this.eventsController = eventsController;
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

        [HttpGet]
        public async Task<IActionResult> Duplicate()
        {
            var result = await eventsController.GetDuplicateEvents();
            if (result is OkObjectResult ok)
            {
                return View(ok.Value);
            }
            else
            {
                ViewBag["Message"] = "Không tìm thấy bất cứ lịch trùng nào!!!";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
