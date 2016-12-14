namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using AspNetCore.Identity;
    using EntityFrameworkCore;
    using Extensions.Logging;
    using global::eShopOnContainers.Identity.Data;
    using global::eShopOnContainers.Identity.Models;
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
                CardHolderName = "Jhon Doe",
                CardNumber = "1111-2222-33-4444",
                CardType = 1,
                City = "Seattle",
                Country = "EEUU",
                CountryCode = "91",
                Email = "jdoe@eshop.com",
                Expiration = "12/20",
                Id = Guid.NewGuid().ToString(), 
                LastName = "Doe", 
                Name = "Jhon", 
                PhoneNumber = "600 11 22 33", 
                UserName = "jdoe@eshop.com", 
                ZipCode = "56730", 
                State = "Washington", 
                Street = "Street..", 
                SecurityNumber = "256", 
                NormalizedEmail = "JDOE@ESHOP.COM", 
                NormalizedUserName = "JDOE@ESHOP.COM", 
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "eshopContainers.123");

            return user;
        }
    }
}
