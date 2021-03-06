﻿using DriveDrop.Api.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;



namespace DriveDrop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                 .MigrateDbContext<ApplicationDbContext>((context, services) =>
                 {
                     var env = services.GetService<IHostingEnvironment>();
                     var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                     var settings = services.GetService<IOptions<AppSettings>>();

                     new ApplicationDbContextSeed()
                         .SeedAsync(context, env, logger, settings)
                         .Wait();
                 })
                .MigrateDbContext<DriveDropContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<DriveDropSettings>>();
                    var logger = services.GetService<ILogger<DriveDropContextSeed>>();

                    new DriveDropContextSeed()
                        .SeedAsync(context, env, settings, logger)
                        .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((context, services ) => {
                    var configuration = services.GetService<IConfiguration>();
                     
                })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args) 
                .UseStartup<Startup>()
               // .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                   // config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .UseApplicationInsights()
                .Build();
    }
}
