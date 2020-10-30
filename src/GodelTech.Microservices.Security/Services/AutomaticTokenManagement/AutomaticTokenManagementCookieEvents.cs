using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GodelTech.Microservices.Security.Services.AutomaticTokenManagement
{
    public class AutomaticTokenManagementCookieEvents : CookieAuthenticationEvents
    {
        private static readonly ConcurrentDictionary<string, bool> PendingRefreshTokenRequests = new ConcurrentDictionary<string, bool>();

        private readonly ITokenEndpointService _service;
        private readonly AutomaticTokenManagementOptions _options;
        private readonly ILogger<AutomaticTokenManagementCookieEvents> _logger;
        private readonly ISystemClock _clock;

        public AutomaticTokenManagementCookieEvents(
            ITokenEndpointService service,
            IOptions<AutomaticTokenManagementOptions> options,
            ILogger<AutomaticTokenManagementCookieEvents> logger,
            ISystemClock clock)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _options = options.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var tokens = (context.Properties.GetTokens() ?? Enumerable.Empty<AuthenticationToken>()).ToArray();
            if (!tokens.Any())
                return;

            var expiresAt = tokens.SingleOrDefault(t => t.Name == "expires_at");
            if (expiresAt == null)
                return;

            var dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
            var dtRefresh = dtExpires.Subtract(_options.RefreshBeforeExpiration);

            if (dtRefresh > _clock.UtcNow)
                return;

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("No refresh token found in cookie properties. A refresh token must be requested and SaveTokens must be enabled.");
                context.RejectPrincipal();
                return;
            }

            var shouldRefresh = PendingRefreshTokenRequests.TryAdd(refreshToken.Value, true);
            if (!shouldRefresh)
                return;

            try
            {
                var response = await _service.RefreshTokenAsync(refreshToken.Value);

                if (response.IsError)
                {
                    _logger.LogWarning("Error refreshing token: {error}", response.Error);
                    return;
                }

                context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, response.AccessToken);
                context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, response.RefreshToken);

                var newExpiresAt = _clock.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
                context.Properties.UpdateTokenValue("expires_at", newExpiresAt.ToString("o", CultureInfo.InvariantCulture));

                await context.HttpContext.SignInAsync(context.Principal, context.Properties);
            }
            finally
            {
                PendingRefreshTokenRequests.TryRemove(refreshToken.Value, out _);
            }
        }

        public override async Task SigningOut(CookieSigningOutContext context)
        {
            if (!_options.RevokeRefreshTokenOnSignout)
                return;

            var result = await context.HttpContext.AuthenticateAsync();

            if (!result.Succeeded)
                return;

            var tokens = (result.Properties.GetTokens() ?? Enumerable.Empty<AuthenticationToken>()).ToArray();
            if (!tokens.Any())
                return;

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
                return;

            var response = await _service.RevokeTokenAsync(refreshToken.Value);
            if (response.IsError)
                _logger.LogWarning("Error revoking token: {error}", response.Error);
        }
    }
}
