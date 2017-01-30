using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Identity.API.Configuration
{
    public class Config
    {
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
        public static IEnumerable<Client> GetClients(Dictionary<string,string> clientsUrl)
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
                    RedirectUris =           { $"{clientsUrl["Spa"]}/" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/" },
                    AllowedCorsOrigins =     { $"{clientsUrl["Spa"]}" },
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
                    RedirectUris =           { "http://eshopxamarin/callback.html" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { "http://13.88.8.119:5105/Account/Redirecting", "http://10.6.1.234:5105/Account/Redirecting" },
                    AllowedCorsOrigins =     { "http://eshopxamarin" },
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
                    ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/signin-oidc",
                        "http://104.40.62.65:5100/signin-oidc", 
                        "http://localhost:5100/signin-oidc",
                        "http://13.88.8.119:5100/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/", 
                        "http://localhost:5100/"
                    },
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