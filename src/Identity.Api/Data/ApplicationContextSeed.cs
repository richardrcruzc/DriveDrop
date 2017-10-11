﻿namespace Identity.Api.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore; 
    using global::Identity.Api.Models;
    using Microsoft.AspNetCore.Builder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class ApplicationContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public ApplicationContextSeed(IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;
            try
            {
                var context = (ApplicationDbContext)applicationBuilder
                    .ApplicationServices.GetService(typeof(ApplicationDbContext));

                context.Database.Migrate();

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        GetDefaultUser());

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;
                    var log = loggerFactory.CreateLogger("catalog seed");
                    log.LogError(ex.Message);
                    await SeedAsync(applicationBuilder, loggerFactory, retryForAvaiability);
                }
            }
        }

        private ApplicationUser GetDefaultUser()
        {
            var user = 
            new ApplicationUser()
            {
                //CardHolderName = "DemoUser",
                //CardNumber = "4012888888881881",
                //CardType = 1,
                //City = "Redmond",
                //Country = "U.S.",
                Email = "admin@driveDrop.com",
                //Expiration = "12/20",
                Id = Guid.NewGuid().ToString(),
                //LastName = "DemoLastName",
                //Name = "DemoUser",
                PhoneNumber = "1234567890",
                UserName = "admin@driveDrop.com",
                //ZipCode = "98052",
                //State = "WA",
                //Street = "15703 NE 61st Ct",
                //SecurityNumber = "535",
                NormalizedEmail = "ADMIN@DRIVEDROP.COM",
                NormalizedUserName = "ADMIN@DRIVEDROP.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed=true
               
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");

            return user;
        }
    }
}
