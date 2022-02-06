using System;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.Services.AutomaticTokenManagement;
using GodelTech.Microservices.Security.UiSecurity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GodelTech.Microservices.Security
{
    /// <summary>
    /// UiSecurity initializer.
    /// </summary>
    public class UiSecurityInitializer : IMicroserviceInitializer
    {
        private readonly string _failurePath;
        private readonly UiSecurityOptions _uiSecurityOptions = new UiSecurityOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="UiSecurityInitializer"/> class.
        /// </summary>
        /// <param name="configureUiSecurity">An <see cref="Action{UiSecurityOptions}"/> to configure the provided <see cref="UiSecurityOptions"/>.</param>
        /// <param name="failurePath">Failure path.</param>
        public UiSecurityInitializer(
            Action<UiSecurityOptions> configureUiSecurity,
            string failurePath = "/Errors/Fault") // todo: a.salanoi: why this path "/Errors/Fault"?
        {
            if (configureUiSecurity == null) throw new ArgumentNullException(nameof(configureUiSecurity));

            _failurePath = failurePath;
            configureUiSecurity.Invoke(_uiSecurityOptions);
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    }
                )
                .AddAutomaticTokenManagement()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, ConfigureOpenIdConnectOptions);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        /// <summary>
        /// Configure OpenIdConnectOptions.
        /// </summary>
        /// <param name="options">OpenIdConnectOptions.</param>
        protected virtual void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

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

            options.SignInScheme = _uiSecurityOptions.SignInScheme;
            options.ResponseType = _uiSecurityOptions.ResponseType;
            options.RequireHttpsMetadata = _uiSecurityOptions.RequireHttpsMetadata;

            // This token can later be found in HttpContext
            options.SaveTokens = _uiSecurityOptions.SaveTokens;
            options.GetClaimsFromUserInfoEndpoint = _uiSecurityOptions.GetClaimsFromUserInfoEndpoint;

            // Apply settings from configuration section
            options.Authority = _uiSecurityOptions.Authority;
            options.ClientId = _uiSecurityOptions.ClientId;
            options.ClientSecret = _uiSecurityOptions.ClientSecret;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _uiSecurityOptions.Issuer
            };

            foreach (var scope in _uiSecurityOptions.Scopes)
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
                    context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(
                        context.ProtocolMessage.IssuerAddress,
                        _uiSecurityOptions.PublicAuthorityUri
                    );

                    return Task.CompletedTask;
                },

                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    context.ProtocolMessage.IssuerAddress = ReplaceDomainAndPort(
                        context.ProtocolMessage.IssuerAddress,
                        _uiSecurityOptions.PublicAuthorityUri
                    );

                    return Task.CompletedTask;
                },

                #endregion

                OnRemoteFailure = context =>
                {
                    context.Response.Redirect(_failurePath);
                    context.HandleResponse();

                    return Task.CompletedTask;
                }
            };
        }

        // todo: make tests and than update to Uri and Helper method
        private static string ReplaceDomainAndPort(string authorityUrl, Uri publicAuthorityUri)
        {
            // todo: remove this after tests
            if (publicAuthorityUri == null) return authorityUrl;
            var publicAuthorityAddress = publicAuthorityUri.AbsoluteUri;
            //

            if (string.IsNullOrWhiteSpace(publicAuthorityAddress))
                return authorityUrl;

            publicAuthorityAddress = publicAuthorityAddress.EndsWith("/") ?
                publicAuthorityAddress.Substring(publicAuthorityAddress.Length - 1) :
                publicAuthorityAddress;

            return publicAuthorityAddress + new Uri(authorityUrl).PathAndQuery;

            //return new Uri(publicAuthorityUri, new Uri(authorityUrl).PathAndQuery).AbsoluteUri;
        }
    }
}