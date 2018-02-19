using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace DriveDrop.Bl.Configuration
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("drivedrop", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                  new Client
                {
                    ClientId = "xamarin",
                    ClientName = "Drivedrop Xamarin OpenId Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                   //AllowedGrantTypes = GrantTypes.Hybrid,
                    // AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                   RedirectUris = { "http://10.0.0.51:5205/xamarincallback" },
                    RequireConsent = false,
                   PostLogoutRedirectUris = {  "http://10.0.0.51:5205/Account/Redirecting" },
                    AllowedCorsOrigins = { "http://drivedropxamarin" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email,
                        "drivedrop",
                        "locations"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true ,
                    AllowAccessTokensViaBrowser  = true
                },
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "drivedrop"}
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "drivedrop"}
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5205/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5205/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "drivedrop"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
