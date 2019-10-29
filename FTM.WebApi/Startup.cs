using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Google.Apis.Auth.AspNetCore;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace FTM.WebApi
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
            #region Common
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #endregion

            #region Reverse proxy support
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            #endregion

            services.Configure<ClientInfo>(Configuration.GetSection("Google"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientInfo>>().Value);
            services.AddDbContext<FtmDbContext>(options => options.UseSqlite(@"Data Source = C:\FtmDatabase.db3"));

            #region Swagger
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MathApp API", Version = "v1" });
            });
            #endregion

            #region Middleware authentication
            services.AddAuthentication(v => {
                v.DefaultAuthenticateScheme = "Cookie";
                v.DefaultChallengeScheme = "Google";
                v.DefaultSignInScheme = "Cookie";
            })
            .AddCookie("Cookie")
            .AddGoogleOpenIdConnect("Google", options =>
            {
                options.ClientId = Configuration["Google:ClientId"];
                options.ClientSecret = Configuration["Google:ClientSecret"];
                options.Scope.Add("profile");
                options.ResponseType = "code id_token";
                options.Authority = "https://accounts.google.com/";
                options.CallbackPath = "/signin-google";
                options.SignedOutCallbackPath = "/signout-callback-google";
                options.RemoteSignOutPath = "/signout-google";
                options.Scope.Add("https://www.googleapis.com/auth/calendar.events.readonly");
                options.Events.OnRedirectToIdentityProvider = (context) =>
                {
                    Debug.WriteLine("***RedirectToIdentityProvider");
                    context.ProtocolMessage.Prompt = "consent";
                    context.ProtocolMessage.SetParameter("access_type", "offline");
                    return Task.FromResult(0);
                };
                options.Events.OnAuthorizationCodeReceived = async (context) =>
                {
                    Debug.WriteLine("***AuthorizationCodeReceived");
                    var redirectUri = "http://localhost:56067/signin-google"; // is not used but required
                    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                        {
                            ClientId = Configuration["Google:ClientId"],
                            ClientSecret = Configuration["Google:ClientSecret"]
                        },
                        DataStore = new FileDataStore("C:\\token.json"),
                    });
                    var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                        context.Principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value,
                        context.ProtocolMessage.Code,
                        redirectUri,
                        CancellationToken.None);
                    if (tokenResponse != null)
                    {
                        context.HandleCodeRedemption(tokenResponse.AccessToken, tokenResponse.IdToken);
                        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
                    }
                };
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FTM API v1");
                });
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
