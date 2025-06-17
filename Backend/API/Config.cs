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
        public static IEnumerable<Client> GetClients(IConfiguration configuration) =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "my-client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenLifetime = 3600, // access token จะมีอายุ 1 ชั่วโมง
                    AbsoluteRefreshTokenLifetime = 2592000, // refresh token จะมีอายุรวมไม่เกิน 30 วัน นับจากวันที่ออก token ครั้งแรก
                    SlidingRefreshTokenLifetime = 604800, // Refresh token จะมีอายุขยายออกไปเรื่อย ๆ ทุกครั้งที่ใช้งาน (7 วันต่อรอบ)
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    // Added openid and profile scopes
                    AllowOfflineAccess = true, // UPDATE public."Clients" SET  "AllowOfflineAccess"="true" WHERE "ClientId"='my-client';
                    AllowedScopes =
                    {
                        configuration["IdentityServer:Audience"] ?? "api1", // Default to api1 if not set
                        "openid",
                        "profile",
                        "role",
                        "offline_access",
                    },
                },
            };

        public static IEnumerable<ApiScope> GetApiScopes(IConfiguration configuration) =>
            new List<ApiScope>
            {
                new ApiScope(configuration["IdentityServer:ApiName"] ?? "api1", "My API")
                {
                    UserClaims = { "Id", "name", "email", "role" },
                },
            };

        public static IEnumerable<ApiResource> GetApiResources(IConfiguration configuration) =>
            new List<ApiResource>
            {
                new ApiResource(configuration["IdentityServer:ApiName"] ?? "api1", "My API")
                {
                    Scopes = { configuration["IdentityServer:ApiName"] ?? "api1" },
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
