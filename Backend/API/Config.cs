using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace API
{
    public static class Config
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "my-client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenLifetime = 1800, // อายุของ access token (วินาที) = 30 นาที
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    // Added openid and profile scopes
                    AllowOfflineAccess = true, // UPDATE public."Clients" SET  "AllowOfflineAccess"="true" WHERE "ClientId"='my-client';
                    AllowedScopes = { "api1", "openid", "profile", "role", "offline_access" },
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API") { UserClaims = { "Id", "name", "email", "role" } },
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API")
                {
                    Scopes = { "api1" },
                    UserClaims = { "name", "email", "role" },
                },
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "demo",
                    Password = "password",
                    Claims =
                    {
                        new System.Security.Claims.Claim(
                            System.Security.Claims.ClaimTypes.Name,
                            "demo"
                        ),
                        new System.Security.Claims.Claim(
                            System.Security.Claims.ClaimTypes.NameIdentifier,
                            "1"
                        ),
                        new System.Security.Claims.Claim(
                            System.Security.Claims.ClaimTypes.Email,
                            "demo@example.com"
                        ),
                    },
                },
            };
        // new TestUser
        // {
        //     SubjectId = "1",
        //     Username = "demo",
        //     Password = "password"
        // }
    };
}
