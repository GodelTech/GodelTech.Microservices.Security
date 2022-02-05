using System;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.Services;
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
        private const string ErrorsFaultPath = "/Errors/Fault";

        public UiSecurityInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddAutomaticTokenManagement()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, ConfigureOpenIdConnectOptions);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (env == null)
                throw new ArgumentNullException(nameof(env));

            app.UseAuthentication();
            app.UseAuthorization();
        }

        protected virtual void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            var config = new UiSecurityConfig();

            Configuration.Bind("UiSecurityConfig", config);

            // todo: remove this
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            options.BackchannelHttpHandler = handler;
            //

            // todo: remove this
            // https://github.com/IdentityServer/IdentityServer4/issues/2925
            // https://github.com/dotnet/aspnetcore/blob/d8381656429addead2e5eb22ba1356abfb419d86/src/Azure/AzureAD/test/FunctionalTests/WebAuthenticationTests.cs#L192-L194
            // CookieContainer doesn't allow cookies from other paths
            options.CorrelationCookie.Path = "/";
            options.NonceCookie.Path = "/";
            //

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.ResponseType = config.ResponseType;
            options.RequireHttpsMetadata = config.RequireHttpsMetadata;

            // This token can later be found in HttpContext
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = config.GetClaimsFromUserInfoEndpoint;

            // Apply settings from configuration section
            options.Authority = config.AuthorityUri;
            options.ClientId = config.ClientId;
            options.ClientSecret = config.ClientSecret;

            options.TokenValidationParameters.ValidIssuer = config.Issuer;

            foreach (var scope in config.Scopes)
            {
                options.Scope.Add(scope);
            }

            options.Events = new OpenIdConnectEvents
            {
                #region Public \ External address overrides

                // This section is required to support scenario when UI sends requests to Identity
                // using internal network but user need to access Identity using external address
                OnRedirectToIdentityProvider = context =>
                {
                    context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(context.ProtocolMessage.IssuerAddress, config.PublicAuthorityUri);

                    return Task.CompletedTask;
                },

                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(context.ProtocolMessage.IssuerAddress, config.PublicAuthorityUri);

                    return Task.CompletedTask;
                },

                #endregion

                OnRemoteFailure = context =>
                {
                    context.Response.Redirect(ErrorsFaultPath);
                    context.HandleResponse();

                    return Task.CompletedTask;
                }
            };
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