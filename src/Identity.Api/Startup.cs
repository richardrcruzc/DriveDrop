using Identity.Api.Certificate; 
using Identity.Api.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Identity.Api.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Identity.Api.Models;
using Identity.Api.Configuration;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace Identity.Api
{
    public class Startup
    {
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

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            //    services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            //    {
            //        config.SignIn.RequireConfirmedEmail = true;
            //    })
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();


            // Add framework services.
            services.AddMvc();


            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ILoginService<Models.ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Adds IdentityServer
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddSigningCredential(Certificate.Certificate.Get())
                .AddAspNetIdentity<Models.ApplicationUser>()
                .AddConfigurationStore(builder =>
                    builder.UseSqlServer(connectionString, options =>
                        options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder =>
                    builder.UseSqlServer(connectionString, options =>
                        options.MigrationsAssembly(migrationsAssembly)))
                .Services.AddTransient<IProfileService, ProfileService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

       //     var options = new RewriteOptions()
       //.AddRedirectToHttps();

       //     app.UseRewriter(options);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            app.UseIdentity();

           
            // Adds IdentityServer
            app.UseIdentityServer();


            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //    AuthenticationScheme = "Google",
            //    DisplayName = "Google",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
               
            //    ClientId = "richardcruz@api-project-382181066575.iam.gserviceaccount.com",
            //    ClientSecret = "89d9dcc18184fce2a18ad482aee492159281f9fb"
            //});

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

            // Store idsrv grant config into db
            InitializeGrantStoreAndConfiguration(app).Wait();

            //Seed Data
            var hasher = new PasswordHasher<ApplicationUser>();
            new ApplicationContextSeed(hasher).SeedAsync(app, loggerFactory).Wait();
        }
        private async Task InitializeGrantStoreAndConfiguration(IApplicationBuilder app)
        {
            //callbacks urls from config:
            Dictionary<string, string> clientUrls = new Dictionary<string, string>();
            clientUrls.Add("Mvc", Configuration.GetValue<string>("MvcClient"));
            clientUrls.Add("Spa", Configuration.GetValue<string>("SpaClient"));
            clientUrls.Add("Xamarin", Configuration.GetValue<string>("XamarinCallback"));

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(clientUrls))
                    {
                        await context.Clients.AddAsync(client.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetResources())
                    {
                        await context.IdentityResources.AddAsync(resource.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var api in Config.GetApis())
                    {
                        await context.ApiResources.AddAsync(api.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
