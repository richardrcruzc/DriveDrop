namespace DriveDrop.Api
{ 
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using DriveDrop.Api.Infrastructure;
    using DriveDrop.Api.Services;
    using global::DriveDrop.Api.Application.IntegrationEvents;
    using global::DriveDrop.Api.Application.IntegrationEvents.Events;
    using global::DriveDrop.Api.Infrastructure.Filters;
    using global::DriveDrop.Api.Infrastructure.HostedServices;
    using global::DriveDrop.Api.Infrastructure.Middlewares; 
    using Infrastructure.AutofacModules; 
    using Infrastructure.Services;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.ServiceFabric;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging; 
    using RabbitMQ.Client;
    using Swashbuckle.AspNetCore.Swagger;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.IdentityModel.Tokens.Jwt;
    using System.Reflection;

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


            var connectionString = Configuration.GetConnectionString("ConnectionString");

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();  //Injecting Controllers themselves thru DI
                                            //For further info see: http://docs.autofac.org/en/latest/integration/aspnetcore.html#controllers-as-services

            // Configure GracePeriodManager Hosted Service
            //services.AddSingleton<IHostedService, GracePeriodManagerService>();

            //services.AddTransient<IShippingIntegrationEventService, ShippingIntegrationEventService>();

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("OrderingDb", connectionString, TimeSpan.FromMinutes(minutes));
            });
            
            services.AddEntityFrameworkSqlServer()
                        .AddDbContext<DriveDropContext>(options =>
                        {
                            options.UseSqlServer(connectionString,
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                        },
                            ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                        );


            //services.AddDbContext<IntegrationEventLogContext>(options =>
            //{
            //    options.UseSqlServer(connectionString,
            //                         sqlServerOptionsAction: sqlOptions =>
            //                         {
            //                             sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                             //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            //                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                         });
            //});

            services.Configure<DriveDropSettings>(Configuration);
            services.Configure<AppSettings>(Configuration);

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "DriveDrop HTTP API",
                    Version = "v1",
                    Description = "The DriveDrop Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "driveDrop", "DriveDrop API" }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IShippingIntegrationEventService, ShippingIntegrationEventService>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IRateService, RateService>();
            services.AddTransient<IDistanceService, DistanceService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IPayPalStandardPaymentProcessor, PayPalStandardPaymentProcessor>();
            services.AddTransient<IGeolocationService, GeolocationService>();


            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = Configuration["EventBusConnection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                    var factory = new ConnectionFactory()
                    {
                        //HostName = Configuration["EventBusConnection"]
                        HostName = "localhost"
                    };

                    if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                    {
                        factory.UserName = Configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                    {
                        factory.Password = Configuration["EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            }


            RegisterEventBus(services);
            ConfigureAuthService(services);
            services.AddOptions();

            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(connectionString));


            return new AutofacServiceProvider(container.Build());

        }



        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            ConfigureAuth(app);
            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "My API V1");
                   c.ConfigureOAuth2("orderingswaggerui", "", "", "Ordering Swagger UI");
               });

            //app.UseHangfireDashboard();
            //app.UseHangfireServer();
            //app.UseHangfireServer();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            //{
            //    Authorization = new[] { new CustomAuthorizeFilter() }
            //});

            ConfigureEventBus(app);
        }

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

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            // var eventBus = app.ApplicationServices.GetRequiredService<Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions.IEventBus>();

            //eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
            //eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
            //eventBus.Subscribe<ShippingStockConfirmedIntegrationEvent, IIntegrationEventHandler<ShippingStockConfirmedIntegrationEvent>>();
            //eventBus.Subscribe<ShippingStockRejectedIntegrationEvent, IIntegrationEventHandler<ShippingStockRejectedIntegrationEvent>>();
            //eventBus.Subscribe<ShippingPaymentFailedIntegrationEvent, IIntegrationEventHandler<ShippingPaymentFailedIntegrationEvent>>();
            //eventBus.Subscribe<ShippingPaymentSuccededIntegrationEvent, IIntegrationEventHandler<ShippingPaymentSuccededIntegrationEvent>>();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "drivedrop";
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            //if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            //{
            //    services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            //    {
            //        var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            //        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            //        var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
            //        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            //        var subscriptionClientName = Configuration["SubscriptionClientName"];

            //        return new EventBusServiceBus(serviceBusPersisterConnection, logger,
            //            eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            //    });
            //}
            //else
            //{
            //    services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            //    {
            //        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            //        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            //        var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            //        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            //        var retryCount = 5;
            //        if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
            //        {
            //            retryCount = int.Parse(Configuration["EventBusRetryCount"]);
            //        }

            //        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, retryCount);
            //    });
            //}

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }





        //// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
        //    IHostingEnvironment _env, IRateService rate, IDistanceService distance, IApplicationLifetime appLifetime)
        //{
        //    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        //    loggerFactory.AddDebug();


        //    app.UseDeveloperExceptionPage();
        //       app.UseBrowserLink();

        //    app.UseHangfireServer();
        //    app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        //    {
        //        Authorization = new[] { new CustomAuthorizeFilter() }
        //    });

        // //   appLifetime.ApplicationStarted.Register(RecurringJob.AddOrUpdate(() => DriveDrop.Api.Services.Auth SendBatchEmailFromQueueAsync(), Cron.Minutely));

        //    // Use frameworks
        //    app.UseCors("CorsPolicy");

        //    ConfigureAuth(app);


        //    app.UseMvcWithDefaultRoute();

        //    app.UseSwagger()
        //       .UseSwaggerUI(c =>
        //       {
        //           c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        //       });


        //    WaitForSqlAvailabilityAsync(loggerFactory, app, _env, rate, distance).Wait();




        //}
        //protected virtual void ConfigureAuth(IApplicationBuilder app)
        //{
        //    var identityUrl = Configuration.GetValue<string>("IdentityUrl");
        //    app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
        //    {
        //        Authority = identityUrl.ToString(),
        //        ApiName = "drivedrop",
        //        RequireHttpsMetadata = false,

        //    });
        //}

        //private async Task WaitForSqlAvailabilityAsync(ILoggerFactory loggerFactory, IApplicationBuilder app, IHostingEnvironment _env, IRateService rate, IDistanceService distance, int retries = 0)
        //{
        //    var logger = loggerFactory.CreateLogger(nameof(Startup));
        //    var policy = CreatePolicy(retries, logger, nameof(WaitForSqlAvailabilityAsync));
        //    await policy.ExecuteAsync(async () =>
        //    {
        //        await DriveDropSeed.SeedAsync(app, loggerFactory, _env, rate, distance);
        //    });

        //}
        //private Policy CreatePolicy(int retries, ILogger logger, string prefix)
        //{
        //    return Policy.Handle<SqlException>().
        //        WaitAndRetryAsync(
        //            retryCount: retries,
        //            sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
        //            onRetry: (exception, timeSpan, retry, ctx) =>
        //            {
        //                logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
        //            }
        //        );
        //}
        //private void RegisterAppInsights(IServiceCollection services)
        //{
        //    services.AddApplicationInsightsTelemetry(Configuration);
        //    var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

        //    if (orchestratorType?.ToUpper() == "K8S")
        //    {
        //        // Enable K8s telemetry initializer
        //        services.EnableKubernetes();
        //    }
        //    if (orchestratorType?.ToUpper() == "SF")
        //    {
        //        // Enable SF telemetry initializer
        //        services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
        //            new FabricTelemetryInitializer());
        //    }
        //}

        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<BuildingBlocks.EventBus.Abstractions.IEventBus>();

        //    eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
        //    eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderPaymentSuccededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSuccededIntegrationEvent>>();
        //}

        //private void ConfigureAuthService(IServiceCollection services)
        //{
        //    // prevent from mapping "sub" claim to nameidentifier.
        //    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        //    var identityUrl = Configuration.GetValue<string>("IdentityUrl");

        //    services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        //    }).AddJwtBearer(options =>
        //    {
        //        options.Authority = identityUrl;
        //        options.RequireHttpsMetadata = false;
        //        options.Audience = "orders";
        //    });
        //}

        //protected virtual void ConfigureAuth(IApplicationBuilder app)
        //{
        //    if (Configuration.GetValue<bool>("UseLoadTest"))
        //    {
        //        app.UseMiddleware<ByPassAuthMiddleware>();
        //    }

        //    app.UseAuthentication();
        //}

        //private void RegisterEventBus(IServiceCollection services)
        //{
        //    if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
        //    {
        //        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        //        {
        //            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
        //            var subscriptionClientName = Configuration["SubscriptionClientName"];

        //            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
        //                eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
        //        });
        //    }
        //    else
        //    {
        //        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        //        {
        //            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //            var retryCount = 5;
        //            if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
        //            {
        //                retryCount = int.Parse(Configuration["EventBusRetryCount"]);
        //            }

        //            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, retryCount);
        //        });
        //    }

        //    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        //}
    }
}
