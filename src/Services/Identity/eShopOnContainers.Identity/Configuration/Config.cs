using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace eShopOnContainers.Identity.Configuration
{
    public class Config
    {
        private readonly IOptions<ClientCallBackUrls> _settings;

        public Config(IOptions<ClientCallBackUrls> settings)
        {
            _settings = settings;
   
        }

        // scopes define the resources in your system
        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                //Authentication OpenId uses this scopes;
                StandardScopes.OpenId,
                StandardScopes.Profile,

                //Each api we want to securice;
                new Scope
                {
                    Name = "orders",
                    Description = "Orders Service"
                },
                new Scope
                {
                    Name = "basket",
                    Description = "Basket Service"
                }
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "eShop SPA OpenId Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        "orders",
                        "basket"
                    }
                }
            };
        }
    }
}