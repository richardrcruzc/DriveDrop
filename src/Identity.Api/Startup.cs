using Identity.Api.Data;
using Identity.Api.Models;
using Identity.Api.Services;
using Identity.Api.Certificate;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;



namespace Identity.Api
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        public IConfiguration Configuration { get; } 
         

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            RegisterAppInsights(services);

            // var connectionString = Configuration.GetConnectionString("ConnectionString");
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(connectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     }));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();


            services.AddIdentity<Models.ApplicationUser, IdentityRole>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;


                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;

                }
                )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.Configure<AppSettings>(Configuration);


            // Add framework services.
            services.AddMvc();

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "drivedrop.identity";
                })
                .PersistKeysToRedis(Configuration["DPConnectionString"]);
            }

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("IdentityDb", connectionString, TimeSpan.FromMinutes(minutes));
            }); 

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ILoginService<Models.ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Adds IdentityServer
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddSigningCredential(Certificate.Certificate.Get())
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(migrationsAssembly);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    });
                })
                .Services.AddTransient<IProfileService, ProfileService>();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }



            app.UseStaticFiles();

            // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            app.UseAuthentication();

           
            // Adds IdentityServer
            app.UseIdentityServer();

             

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
  name: "passwordrecovery",
  template: "passwordrecovery",
  defaults: new { controller = "account", action = "ForgotPassword" });


            });

            //// Store idsrv grant config into db
            //InitializeGrantStoreAndConfiguration(app).Wait();

            //Seed Data
            //var hasher = new PasswordHasher<ApplicationUser>();
            //new ApplicationContextSeed(hasher).SeedAsync(app, loggerFactory).Wait();
        }
        //private async Task InitializeGrantStoreAndConfiguration(IApplicationBuilder app)
        //{
        //    //callbacks urls from config:
        //    Dictionary<string, string> clientUrls = new Dictionary<string, string>();
        //    clientUrls.Add("Mvc", Configuration.GetValue<string>("MvcClient"));
        //    clientUrls.Add("Spa", Configuration.GetValue<string>("SpaClient"));
        //    clientUrls.Add("Xamarin", Configuration.GetValue<string>("XamarinCallback"));

        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        //        context.Database.Migrate();

        //        if (!context.Clients.Any())
        //        {
        //            foreach (var client in Config.GetClients(clientUrls))
        //            {
        //                await context.Clients.AddAsync(client.ToEntity());
        //            }
        //            await context.SaveChangesAsync();
        //        }

        //        if (!context.IdentityResources.Any())
        //        {
        //            foreach (var resource in Config.GetResources())
        //            {
        //                await context.IdentityResources.AddAsync(resource.ToEntity());
        //            }
        //            await context.SaveChangesAsync();
        //        }

        //        if (!context.ApiResources.Any())
        //        {
        //            foreach (var api in Config.GetApis())
        //            {
        //                await context.ApiResources.AddAsync(api.ToEntity());
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //}

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.EnableKubernetes();
            }
            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }
        }
    }
}
