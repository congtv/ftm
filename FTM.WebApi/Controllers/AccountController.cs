using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
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

        public AccountController(ILogger<AccountController> logger, FtmDbContext context, IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
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
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookie");

            // create principal
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                    scheme: "Cookie",
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
                    scheme: "Cookie");

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
    }
}
