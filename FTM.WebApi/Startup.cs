using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FTM.WebApi.Models;
using Google.Apis.Auth.AspNetCore;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<GoogleInfo>(Configuration.GetSection("Google"));
        
        services.AddAuthentication(v => {
                v.DefaultAuthenticateScheme = "Cookie";
                v.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                v.DefaultSignInScheme = "Cookie";
            })
            .AddCookie("Cookie")
            .AddGoogleOpenIdConnect("Google", options =>
            {
                var clientInfo = (ClientInfo)services.First(x => x.ServiceType == typeof(ClientInfo)).ImplementationInstance;
                options.ClientId = clientInfo.ClientId;
                options.ClientSecret = clientInfo.ClientSecret;
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
                options.Events.OnRemoteFailure = (context) =>
                {
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
                            ClientId = "763053086185-an7kopev3msfad5bdv6cp89srovc1g7f.apps.googleusercontent.com",
                            ClientSecret = "YUVJEaLeyHgjnItpO2LH2LJe"
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
