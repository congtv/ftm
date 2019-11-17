﻿using FTM.WebApi.Common;
using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #endregion Common

            #region Reverse proxy support

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            #endregion Reverse proxy support

            services.Configure<ClientInfo>(Configuration.GetSection("Google"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientInfo>>().Value);

            services.AddScoped(typeof(FtmDataStore));

            services.AddDbContext<FtmDbContext>(options => options.UseSqlite(Configuration["DefaultConnection"]));

            #region Swagger

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Free Time Manager API", Version = "v1" });
            });

            #endregion Swagger

            #region Middleware authentication

            var key = Encoding.ASCII.GetBytes(Configuration["Settings:Jwt:Key"]);
            services.AddAuthentication(v =>
            {
                v.DefaultAuthenticateScheme = "Cookie";
                v.DefaultChallengeScheme = "Google";
                v.DefaultSignInScheme = "Cookie";
                v.DefaultSignOutScheme = "Cookie";
            })
            .AddCookie("Cookie", op =>
            {
                op.ExpireTimeSpan = TimeSpan.FromSeconds(int.TryParse(Configuration["Settings:CookieExpireSecond"], out int expireTime) ? expireTime : 30); ;
                op.SlidingExpiration = true;
                op.LogoutPath = "/account/logout";
                op.LoginPath = "/account/login";
            })
            .AddJwtBearer("Bearer", x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            })
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
                options.Scope.Add("https://www.googleapis.com/auth/calendar");
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

                    var email = context.JwtSecurityToken.Claims.First(x => x.Type == "email");
                    if (email.Value == Configuration["Settings:AdminEmail"])
                    {
                        // is not used but required
                        string redirectUri;
                        if (Enviroment.IsDevelopment())
                        {
                            redirectUri = "http://localhost:56067/signin-google";
                        }
                        else
                        {
                            if(context.Request.Scheme == "https")
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
            else
            {
                //// Config for reverse proxy use subpath MathAppApi
                //app.UsePathBase("/ftm");
                //app.UseHsts();
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Lỗi rồi nhé, chắc là đăng nhập sai thôi!!!", encoding: Encoding.Unicode);
                    });
                });
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FTM API v1");
                });
            }
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}