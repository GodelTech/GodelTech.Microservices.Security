using System;
using System.Threading.Tasks;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.Helpers;
using GodelTech.Microservices.Security.UiSecurity;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
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
        private readonly Action<AccessTokenManagementOptions> _configureAccessTokenManagement;

        /// <summary>
        /// Initializes a new instance of the <see cref="UiSecurityInitializer"/> class.
        /// </summary>
        /// <param name="configureUiSecurity">An <see cref="Action{UiSecurityOptions}"/> to configure the provided <see cref="UiSecurityOptions"/>.</param>
        /// <param name="configureAccessTokenManagement">An <see cref="Action{AccessTokenManagementOptions}"/> to configure the provided <see cref="AccessTokenManagementOptions"/>.</param>
        /// <param name="failurePath">Failure path.</param>
        public UiSecurityInitializer(
            Action<UiSecurityOptions> configureUiSecurity,
            Action<AccessTokenManagementOptions> configureAccessTokenManagement = null,
            string failurePath = "/Errors/Fault")
        {
            if (configureUiSecurity == null) throw new ArgumentNullException(nameof(configureUiSecurity));

            _failurePath = failurePath;

            configureUiSecurity.Invoke(_uiSecurityOptions);
            _configureAccessTokenManagement = configureAccessTokenManagement;
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
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options => options.Events.OnSigningOut = async context =>
                    {
                        await context.HttpContext.RevokeUserRefreshTokenAsync();
                    }
                )
                .AddOpenIdConnect(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    ConfigureOpenIdConnectOptions
                );

            // adds user and client access token management
            services
                .AddAccessTokenManagement(_configureAccessTokenManagement)
                .ConfigureBackchannelHttpClient();
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        /// <summary>
        /// Configure OpenIdConnectOptions.
        /// </summary>
        /// <param name="options">OpenIdConnectOptions.</param>
        protected virtual void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Authority = _uiSecurityOptions.Authority;

            options.ClientId = _uiSecurityOptions.ClientId;
            options.ClientSecret = _uiSecurityOptions.ClientSecret;

            options.GetClaimsFromUserInfoEndpoint = _uiSecurityOptions.GetClaimsFromUserInfoEndpoint;
            options.RequireHttpsMetadata = _uiSecurityOptions.RequireHttpsMetadata;
            options.ResponseType = _uiSecurityOptions.ResponseType;

            options.Scope.Clear();
            foreach (var scope in _uiSecurityOptions.Scopes)
            {
                options.Scope.Add(scope);
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _uiSecurityOptions.Issuer
            };

            options.UsePkce = _uiSecurityOptions.UsePkce;

            options.Events = CreateOpenIdConnectEvents();

            options.SaveTokens = _uiSecurityOptions.SaveTokens;
        }

        /// <summary>
        /// Create OpenIdConnectEvents.
        /// </summary>
        /// <returns>OpenIdConnectEvents.</returns>
        protected virtual OpenIdConnectEvents CreateOpenIdConnectEvents()
        {
            var events = new OpenIdConnectEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect(_failurePath);
                    context.HandleResponse();

                    return Task.CompletedTask;
                }
            };

            if (_uiSecurityOptions.PublicAuthorityUri == null) return events;

            #region Public \ External address overrides

            // This section is required to support scenario when UI sends requests to Identity
            // using internal network but user need to access Identity using external address
            events.OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.IssuerAddress = UrlHelpers.ChangeAuthority(
                    context.ProtocolMessage.IssuerAddress,
                    _uiSecurityOptions.PublicAuthorityUri
                );

                return Task.CompletedTask;
            };

            events.OnRedirectToIdentityProviderForSignOut = context =>
            {
                context.ProtocolMessage.IssuerAddress = UrlHelpers.ChangeAuthority(
                    context.ProtocolMessage.IssuerAddress,
                    _uiSecurityOptions.PublicAuthorityUri
                );

                return Task.CompletedTask;
            };

            #endregion

            return events;
        }
    }
}
