#define UseOptions // or NoOptions
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;  
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels; 
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.eShopOnContainers.BuildingBlocks; 
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.HealthChecks;
using Microsoft.AspNetCore; 
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DriveDrop.Web.WebSocketManager;

namespace DriveDrop.Web
{
    public class Startup
    {
       // private IServiceCollection _services;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();


            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly);
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc();
           
            // services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "drivedrop.webmvc";
                })
                .PersistKeysToRedis(Configuration["DPConnectionString"]);
            }

            services.Configure<AppSettings>(Configuration);

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
                ConfigurationOptions configuration = ConfigurationOptions.Parse(settings.RediConnectionString, true);
                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddUrlCheck(Configuration["DriveDropUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["IdentityUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IRatingRepository, RedisRatingRepository>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            services.AddTransient<FormatService>();


            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }


            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //app.Map("/ws", SocketHandler.Map);

            var wsOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(wsOptions);
            app.UseMiddleware<ChatWebSocketMiddleware>();
            //app.UseMiddleware<SocketMiddleware>();

//#if NoOptions
//            #region UseWebSockets
//                        app.UseWebSockets();
//            #endregion
//#endif
//#if UseOptions
//            #region UseWebSocketsOptions
//            var webSocketOptions = new WebSocketOptions()
//            {
//                KeepAliveInterval = TimeSpan.FromSeconds(120),
//                ReceiveBufferSize = 4 * 1024
//            };
//            app.UseWebSockets(webSocketOptions);
//            #endregion
//#endif
//            #region AcceptWebSocket
//            app.Use(async (context, next) =>
//            {
//                if (context.Request.Path == "/ws")
//                {
//                    if (context.WebSockets.IsWebSocketRequest)
//                    {
//                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
//                        await Echo(context, webSocket);
//                    }
//                    else
//                    {
//                        context.Response.StatusCode = 400;
//                    }
//                }
//                else
//                {
//                    await next();
//                }

//            });
//            #endregion

            app.UseDeveloperExceptionPage();

            app.UseBrowserLink();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
            });

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = Configuration.GetValue<string>("CallBackUrl");
            var log = loggerFactory.CreateLogger("identity");

            var oidcOptions = new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",
                Authority = identityUrl.ToString(),
                PostLogoutRedirectUri = callBackUrl.ToString(),
                ClientId = "mvc",
                ClientSecret = "secret",
                ResponseType = "code id_token",
                SaveTokens = true,
                GetClaimsFromUserInfoEndpoint = true,
                RequireHttpsMetadata = false,
                Scope = { "openid", "profile", "drivedrop" }
            };

            //Wait untill identity service is ready on compose. 
            app.UseOpenIdConnectAuthentication(oidcOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=Index}/{id?}");
            });

            //app.UseSignalR(r => r.MapHub<NotificationHub>("/notification"));
        }
        #region Echo
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion
    }
}
