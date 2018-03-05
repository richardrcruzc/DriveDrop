using DriveDrop.Bl.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;


namespace DriveDrop.Bl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                 .MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
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
                  .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                  {
                      var configuration = services.GetService<IConfiguration>();

                      new ConfigurationDbContextSeed()
                          .SeedAsync(context, configuration)
                          .Wait();
                  })
                 .Run();
        }
      
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           .UseShutdownTimeout(TimeSpan.FromSeconds(10))
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
