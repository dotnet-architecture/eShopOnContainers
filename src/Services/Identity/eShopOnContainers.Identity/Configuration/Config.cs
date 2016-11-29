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
                },
                new Client
                {
                    ClientId = "xamarin",
                    ClientName = "eShop Xamarin OpenId Client",
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
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientUri = "http://localhost:2114",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:2114/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:2114/"
                    },
                    LogoutUri = "http://localhost:2114/signout-oidc",
                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.OfflineAccess.Name,
                        "orders",
                        "basket",
                    },
                }
            };
        }
    }
}