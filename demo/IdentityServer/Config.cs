using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =
            new List<ApiScope>
            {
                new ApiScope("api", "API"),
                new ApiScope("fake.add", "Fake Add"),
                new ApiScope("fake.edit", "Fake Edit"),
                new ApiScope("fake.delete", "Fake Delete"),
                new ApiScope("fake.unused", "Fake unused scope")
            };

        public static IEnumerable<ApiResource> ApiResource =
            new List<ApiResource>
            {
                new ApiResource("DemoApi", "API")
                {
                    Scopes =
                    {
                        "api",
                        "fake.add",
                        "fake.edit",
                        "fake.delete",
                        "fake.unused"
                    }
                }
            };

        public static IEnumerable<Client> Clients =
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "ClientForApi",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes =
                    {
                        "api",
                        "fake.add",
                        "fake.edit",
                        "fake.delete",
                        "fake.unused"
                    }
                },
                // interactive ASP.NET Core Mvc client
                new Client
                {
                    ClientId = "Mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.AuthorizationCode,
                        GrantType.ClientCredentials
                    },

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:44302/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:44302/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api"
                    },

                    AllowOfflineAccess = true
                },
                // interactive ASP.NET Core RazorPages client
                new Client
                {
                    ClientId = "RazorPages",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.AuthorizationCode,
                        GrantType.ClientCredentials
                    },
                    
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:44303/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:44303/signout-callback-oidc" },
                    
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api"
                    },

                    AllowOfflineAccess = true
                }
            };

        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                var users = IdentityServerHost.Quickstart.UI.TestUsers.Users;

                users.Add(
                    new TestUser
                    {
                        SubjectId = "007",
                        Username = "test@example.com",
                        Password = "Secret1234!",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "John Doe"),
                            new Claim(JwtClaimTypes.GivenName, "John"),
                            new Claim(JwtClaimTypes.FamilyName, "Doe"),
                            new Claim(JwtClaimTypes.Email, "test@example.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://JohnDoe.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    }
                );

                return users;
            }
        }
    }
}