using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class RazorPagesUiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;

        public RazorPagesUiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _httpClientHandler = HttpClientHelpers.CreateHttpClientHandler();

            _httpClient = HttpClientHelpers.CreateHttpClient(
                _httpClientHandler,
                _fixture.RazorPagesApplication.Url
            );
        }

        public void Dispose()
        {
            _httpClientHandler.Dispose();
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

        // todo: fix this test
        [Fact]
        public async Task SecuredPageRequested_Success()
        {
            // Arrange
            var cookieContainer = new CookieContainer();
            using var httpClientHandler2 = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                CookieContainer = cookieContainer
            };
            using var client = HttpClientHelpers.CreateClient(httpClientHandler2, cookieContainer);
            client.BaseAddress = _fixture.RazorPagesApplication.Url;

            // Arrange & Act
            var result = await A(cookieContainer, client, new Uri("User", UriKind.Relative));
            //var a = await result.Content.ReadAsStringAsync();
            //var result = await client.GetAsync(new Uri("User", UriKind.Relative));

            // Assert
            Assert.Equal(
                _fixture.RazorPagesApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/User", result.RequestMessage.RequestUri.AbsolutePath);
        }

        private static async Task<HttpResponseMessage> A(CookieContainer cookieContainer, HttpClient client, Uri url)
        {
            var response = await client.GetAsync(url);
            var cookies = cookieContainer.GetCookies(new Uri("https://localhost:44300/connect/authorize"));
            cookieContainer.Add(cookies);

            var content = await response.Content.ReadAsStringAsync();

            var code = string.Empty;
            var scope = string.Empty;
            var state = string.Empty;
            var sessionState = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                code = content.Substring(content.IndexOf("name='code'", StringComparison.InvariantCulture));
                code = code.Substring(code.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                code = code.Substring(0, code.IndexOf("'", StringComparison.InvariantCulture));

                scope = content.Substring(content.IndexOf("name='scope'", StringComparison.InvariantCulture));
                scope = scope.Substring(scope.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                scope = scope.Substring(0, scope.IndexOf("'", StringComparison.InvariantCulture));

                state = content.Substring(content.IndexOf("name='state'", StringComparison.InvariantCulture));
                state = state.Substring(state.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                state = state.Substring(0, state.IndexOf("'", StringComparison.InvariantCulture));

                sessionState = content.Substring(content.IndexOf("name='session_state'", StringComparison.InvariantCulture));
                sessionState = sessionState.Substring(sessionState.IndexOf("value='", StringComparison.InvariantCulture) + 7);
                sessionState = sessionState.Substring(0, sessionState.IndexOf("'", StringComparison.InvariantCulture));
            }

            using var contentToSend = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("state", state),
                new KeyValuePair<string, string>("session_state", sessionState)
            });
            var response2 = await client.PostAsync(new Uri("https://localhost:44303/signin-oidc"), contentToSend);

            return response2;
        }
    }
}