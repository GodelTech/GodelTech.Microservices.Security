using System;
using System.Threading.Tasks;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.Services.AutomaticTokenManagement;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security
{
    public class UiSecurityInitializer : MicroserviceInitializerBase
    {
        public UiSecurityInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var identityConfig = Configuration.GetIdentityConfiguration();
            services.AddSingleton(identityConfig);

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddAutomaticTokenManagement()
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = "code id_token";
                    options.RequireHttpsMetadata = false;

                    // This token can later be found in HttpContext
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // Apply settings from configuration section
                    options.Authority = identityConfig.AuthorityUri;
                    options.ClientId = identityConfig.ClientId;
                    options.ClientSecret = identityConfig.ClientSecret;

                    options.TokenValidationParameters.ValidIssuer = identityConfig.Issuer;
                    options.Scope.AddRange(identityConfig.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries));

                    options.Events = new OpenIdConnectEvents
                    {
                        #region Public \ External address overrides

                        // This section is required to support scenario when UI sends requests to Identity
                        // using internal network but user need to access Identity using external address
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(context.ProtocolMessage.IssuerAddress, identityConfig.PublicAuthorityUri);

                            return Task.CompletedTask;
                        },

                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(context.ProtocolMessage.IssuerAddress, identityConfig.PublicAuthorityUri);

                            return Task.CompletedTask;
                        },

                        #endregion

                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/Errors/Fault");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private static string ReplaceDomainAndPort(string authorityUrl, string publicAuthorityAddress)
        {
            if (string.IsNullOrWhiteSpace(publicAuthorityAddress))
                return authorityUrl;

            publicAuthorityAddress = publicAuthorityAddress.EndsWith("/") ?
                publicAuthorityAddress.Substring(publicAuthorityAddress.Length - 1) :
                publicAuthorityAddress;

            return publicAuthorityAddress + new Uri(authorityUrl).PathAndQuery;
        }
    }
}