using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Http;
using ApplicationCore.Interfaces;
using Infrastructure.FileSystem;
using Infrastructure.Logging;
using DriveDrop.Api.Infrastructure;

namespace DriveDrop.Api
{
    public class Startup
    {
        private IServiceCollection _services;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Requires LocalDB which can be installed with SQL Server Express 2016
            // https://www.microsoft.com/en-us/download/details.aspx?id=54284
            services.AddDbContext<DriveDropContext>(c =>
            {
                try
                {
                    //c.UseInMemoryDatabase("Catalog");
                    c.UseSqlServer(Configuration.GetConnectionString("DriveDropConnection"));
                    c.ConfigureWarnings(wb =>
                    {
                        //By default, in this application, we don't want to have client evaluations
                        wb.Log(RelationalEventId.QueryClientEvaluationWarning);
                    });
                }
                catch (System.Exception ex)
                {
                    var message = ex.Message;
                }
            });


            //services.Configure<AppSettings>(Configuration);

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                //options.UseInMemoryDatabase("Identity"));
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();
            // services.AddScoped<ICustomerService, CachedCustomerService>();
            //services.AddScoped<ICustomerService, CustomerService>();
            //services.AddScoped<CustomerService>(); 
            //services.Configure<DriveDropSettings>(Configuration);
            services.AddSingleton<IImageService, LocalFileImageService>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddMvc();



            // Add application services.
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();


            _services = services;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
