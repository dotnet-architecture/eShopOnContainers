namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using AspNetCore.Identity;
    using EntityFrameworkCore;
    using Extensions.Logging;
    using global::Identity.API.Data;
    using global::Identity.API.Models;
    using Microsoft.AspNetCore.Builder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

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
                CardHolderName = "DemoUser",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Email = "demouser@microsoft.com",
                Expiration = "12/20",
                Id = Guid.NewGuid().ToString(), 
                LastName = "DemoLastName", 
                Name = "DemoUser", 
                PhoneNumber = "1234567890", 
                UserName = "demouser@microsoft.com", 
                ZipCode = "98052", 
                State = "WA", 
                Street = "15703 NE 61st Ct", 
                SecurityNumber = "535", 
                NormalizedEmail = "DEMOUSER@MICROSOFT.COM", 
                NormalizedUserName = "DEMOUSER@MICROSOFT.COM", 
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");

            return user;
        }
    }
}
