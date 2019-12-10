using FTM.WebApi.Common;
using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTM.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Enviroment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Enviroment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Common

            services.AddCors();
            services.AddMvc().AddControllersAsServices().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            #endregion Common

            services.Configure<ClientInfo>(Configuration.GetSection("Google"));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientInfo>>().Value);

            services.AddScoped(typeof(FtmDataStore));

            services.AddDbContext<FtmDbContext>(options => options.UseSqlite(Configuration["DefaultConnection"]));

            #region Middleware authentication

            var key = Encoding.ASCII.GetBytes(Configuration["Settings:Jwt:Key"]);
            services.AddAuthentication(v =>
            {
                v.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                v.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                v.DefaultChallengeScheme = "Google";
                v.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies", op =>
            {
                op.ExpireTimeSpan = TimeSpan.FromSeconds(int.TryParse(Configuration["Settings:CookieExpireMinues"], out int expireTime) ? expireTime : 30);
                op.SlidingExpiration = true;
                op.LogoutPath = "/account/logout";
                op.LoginPath = "/account/login";
                op.Events = new CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = context =>
                    {
                        var isReauthenGoogle = context.Request.Path == "/Account/Authenticate";
                        if (isReauthenGoogle)
                        {
                            context.Request.Path = new PathString("/account/authenticate/success");
                            context.RejectPrincipal();
                        }
                        return Task.CompletedTask;
                    },
                };
            })
            .AddGoogleOpenIdConnect("Google", options =>
            {
                options.ClientId = Configuration["Google:ClientId"];
                options.ClientSecret = Configuration["Google:ClientSecret"];
                options.ResponseType = "code id_token";
                options.Authority = "https://accounts.google.com/";
                options.CallbackPath = "/signin-google";
                options.SignedOutCallbackPath = "/signout-callback-google";
                options.RemoteSignOutPath = "/signout-google";
                options.Scope.Add("https://www.googleapis.com/auth/calendar");
                options.Events.OnRedirectToIdentityProvider = (context) =>
                {
                    Debug.WriteLine("***RedirectToIdentityProvider");
                    context.ProtocolMessage.Prompt = "consent";
                    context.ProtocolMessage.SetParameter("access_type", "offline");
                    return Task.CompletedTask;
                };
                options.Events.OnAuthorizationCodeReceived = async (context) =>
                {
                    Debug.WriteLine("***AuthorizationCodeReceived");

                    var email = context.JwtSecurityToken.Claims.First(x => x.Type == "email");
                    if (email.Value == Configuration["Settings:AdminEmail"])
                    {
                        string redirectUri;
                        if (Enviroment.IsDevelopment())
                        {
                            redirectUri = "http://localhost:56067/signin-google";
                        }
                        else
                        {
                            if (context.Request.Scheme == "https")
                            {
                                redirectUri = $"{this.Configuration["Settings:UrlSchemaHttps"]}signin-google";
                            }
                            else
                            {
                                redirectUri = $"{this.Configuration["Settings:UrlSchemaHttp"]}signin-google";
                            }
                        }
                        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                            {
                                ClientId = Configuration["Google:ClientId"],
                                ClientSecret = Configuration["Google:ClientSecret"]
                            },
                            DataStore = new FtmDataStore(new FtmDbContext(Configuration["DefaultConnection"])),
                        });
                        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                            Constains.UserId,
                            context.ProtocolMessage.Code,
                            redirectUri,
                            CancellationToken.None);
                        if (tokenResponse != null)
                        {
                            context.HandleCodeRedemption(tokenResponse.AccessToken, tokenResponse.IdToken);
                            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                };
            });

            #endregion Middleware authentication
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UsePathBase("/ftm");
                app.UseHsts();
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Lỗi rồi nhé, chắc là đăng nhập sai thôi!!!", encoding: Encoding.Unicode);
                    });
                });
            }
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}