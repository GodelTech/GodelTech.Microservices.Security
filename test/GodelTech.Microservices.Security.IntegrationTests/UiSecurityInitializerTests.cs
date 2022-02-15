using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public abstract class UiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly Uri _applicationUrl;

        private readonly HttpClient _httpClient;
        private readonly HttpClient _userHttpClient;

        private bool _isDisposed;

        protected UiSecurityInitializerTests(
            TestFixture fixture,
            Uri applicationUrl,
            string httpClientName,
            string secondHttpClientName)
        {
            _fixture = fixture;

            var httpClientFactory = _fixture.ServiceProvider.GetService<IHttpClientFactory>();

            _applicationUrl = applicationUrl;

            _httpClient = httpClientFactory.CreateClient(httpClientName);
            _userHttpClient = httpClientFactory.CreateClient(secondHttpClientName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task SecuredPageRequested_RedirectsToIdentityServerLoginPage()
        {
            // Arrange & Act
            var result = await _httpClient.GetAsync(new Uri("User", UriKind.Relative));

            // Assert
            Assert.Equal(
                _fixture.IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task SecuredPageRequested_WhenUserLoggedIn_ReturnsUserPage()
        {
            // Arrange
            await LoginUiClientAsync(_userHttpClient, "alice", "alice");

            // Act
            var result = await GetAsync(
                _userHttpClient,
                new Uri("User", UriKind.Relative)
            );

            // Assert
            Assert.Equal(
                _applicationUrl.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/User", result.RequestMessage.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task SecuredPageRequested_WhenUserLoggedOutIdentityServer_RedirectsToIdentityServerLoginPage()
        {
            // Arrange
            await LoginUiClientAsync(_userHttpClient, "alice", "alice");

            await GetAsync(
                _userHttpClient,
                new Uri("User", UriKind.Relative)
            );

            await LogoutUiClientAsync(_userHttpClient);

            // Arrange
            var result = await _httpClient.GetAsync(new Uri("User", UriKind.Relative));

            // Assert
            Assert.Equal(
                _fixture.IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task SecuredPageRequested_WhenUserLoggedOut_RedirectsToIdentityServerLoginPage()
        {
            // Arrange
            await LoginUiClientAsync(_userHttpClient, "alice", "alice");

            await GetAsync(
                _userHttpClient,
                new Uri("User", UriKind.Relative)
            );

            await _httpClient.GetAsync(new Uri("Logout", UriKind.Relative));

            // Arrange
            var result = await _httpClient.GetAsync(new Uri("User", UriKind.Relative));

            // Assert
            Assert.Equal(
                _fixture.IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                // unmanaged resources would be cleaned up here.
                return;
            }

            if (_isDisposed)
            {
                // no need to dispose twice.
                return;
            }

            // free managed resources 
            _userHttpClient.Dispose();
            _httpClient.Dispose();
            _isDisposed = true;
        }

        public async Task LoginUiClientAsync(HttpClient httpClient, string username, string password)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var loginUrl = new Uri(_fixture.IdentityServerApplication.Url, "Account/Login");

            var response = await httpClient.GetAsync(loginUrl);
            var responseValue = await response.Content.ReadAsStringAsync();

            var returnUrl = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(responseValue))
            {
                returnUrl = responseValue.Substring(responseValue.IndexOf("ReturnUrl", StringComparison.InvariantCulture));
                returnUrl = returnUrl.Substring(returnUrl.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("\"", StringComparison.InvariantCulture));

                verificationToken = responseValue.Substring(responseValue.IndexOf("__RequestVerificationToken", StringComparison.InvariantCulture));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\"", StringComparison.InvariantCulture));
            }

            using var contentToSend = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("ReturnUrl", returnUrl),
                    new KeyValuePair<string, string>("Username", username),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("button", "login"),
                    new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
                }
            );

            await httpClient.PostAsync(loginUrl, contentToSend);
        }

        private async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, Uri url)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var response = await httpClient.GetAsync(url);
            if (response.RequestMessage.RequestUri == new Uri(_applicationUrl, url)) return response;

            var responseValue = await response.Content.ReadAsStringAsync();

            var code = string.Empty;
            var scope = string.Empty;
            var state = string.Empty;
            var sessionState = string.Empty;
            if (!string.IsNullOrEmpty(responseValue))
            {
                code = responseValue.Substring(responseValue.IndexOf("name='code'", StringComparison.InvariantCulture));
                code = code.Substring(code.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                code = code.Substring(0, code.IndexOf("'", StringComparison.InvariantCulture));

                scope = responseValue.Substring(responseValue.IndexOf("name='scope'", StringComparison.InvariantCulture));
                scope = scope.Substring(scope.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                scope = scope.Substring(0, scope.IndexOf("'", StringComparison.InvariantCulture));

                state = responseValue.Substring(responseValue.IndexOf("name='state'", StringComparison.InvariantCulture));
                state = state.Substring(state.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                state = state.Substring(0, state.IndexOf("'", StringComparison.InvariantCulture));

                sessionState = responseValue.Substring(responseValue.IndexOf("name='session_state'", StringComparison.InvariantCulture));
                sessionState = sessionState.Substring(sessionState.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                sessionState = sessionState.Substring(0, sessionState.IndexOf("'", StringComparison.InvariantCulture));
            }

            using var postContent = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("scope", scope),
                    new KeyValuePair<string, string>("state", state),
                    new KeyValuePair<string, string>("session_state", sessionState)
                }
            );

            return await httpClient.PostAsync(
                new Uri("signin-oidc", UriKind.Relative),
                postContent
            );
        }

        public async Task LogoutUiClientAsync(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var logoutUrl = new Uri(_fixture.IdentityServerApplication.Url, "Account/Logout");

            var response = await httpClient.GetAsync(logoutUrl);
            var responseValue = await response.Content.ReadAsStringAsync();

            var logoutId = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(responseValue))
            {
                logoutId = responseValue.Substring(responseValue.IndexOf("logoutId", StringComparison.InvariantCulture));
                logoutId = logoutId.Substring(logoutId.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                logoutId = logoutId.Substring(0, logoutId.IndexOf("\"", StringComparison.InvariantCulture));

                verificationToken = responseValue.Substring(responseValue.IndexOf("__RequestVerificationToken", StringComparison.InvariantCulture));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\"", StringComparison.InvariantCulture));
            }

            using var contentToSend = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("LogoutId", logoutId),
                    new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
                }
            );

            await httpClient.PostAsync(logoutUrl, contentToSend);
        }
    }
}