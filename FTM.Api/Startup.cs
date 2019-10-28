using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Google;

namespace FTM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthentication(v => {
                v.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                v.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
                var clientInfo = (ClientInfo)services.First(x => x.ServiceType == typeof(ClientInfo)).ImplementationInstance;
                options.ClientId = clientInfo.ClientId;
                options.ClientSecret = clientInfo.ClientSecret;
                options.Scope.Add("profile");
                ////IConfigurationSection googleAuthNSection =
                ////Configuration.GetSection("Authentication:Google");
                ////options.ClientId = googleAuthNSection["ClientId"];
                ////options.ClientSecret = googleAuthNSection["ClientSecret"];
                //options.ClientId = "763053086185-an7kopev3msfad5bdv6cp89srovc1g7f.apps.googleusercontent.com";
                //options.ClientSecret = "YUVJEaLeyHgjnItpO2LH2LJe";
                ////options.CallbackPath = "/signin-google";
                //options.AccessType = "offline";
                //options.SaveTokens = true;
                ////options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/authorize";
                //options.Events.OnCreatingTicket = context =>
                //{
                //    return Task.FromResult(0);
                //};
                //options.Events.OnTicketReceived = context =>
                //{
                //    return Task.FromResult(0);
                //};
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
