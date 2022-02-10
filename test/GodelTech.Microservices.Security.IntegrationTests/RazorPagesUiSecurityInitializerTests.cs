using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class RazorPagesUiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClient _httpClient;
        private readonly HttpClient _userHttpClient;

        public RazorPagesUiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            var httpClientFactory = _fixture.ServiceProvider.GetService<IHttpClientFactory>();

            _httpClient = httpClientFactory.CreateClient("RazorPagesClient");
            _userHttpClient = httpClientFactory.CreateClient("RazorPagesSecondClient");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
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
        public async Task SecuredPageRequested_Success()
        {
            // Arrange
            await HttpClientHelpers.AuthorizeClientAsync(_userHttpClient, _fixture.IdentityServerApplication.Url);

            // Arrange & Act
            var result = await GetAsync(
                _userHttpClient,
                new Uri("User", UriKind.Relative)
            );

            // Assert
            Assert.Equal(
                _fixture.RazorPagesApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/User", result.RequestMessage.RequestUri.AbsolutePath);
        }

        private static async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, Uri url)
        {
            var response = await httpClient.GetAsync(url);
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
    }
}