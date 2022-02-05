using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
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

            Assert.Matches(
                new Regex(
                    "^" +
                    await File.ReadAllTextAsync("Documents/AccountLoginHtml.txt") +
                    "$"
                ),
                await result.Content.ReadAsStringAsync()
            );
        }

        // todo: fix this test
        [Fact]
        public async Task SecuredPageRequested_2()
        {
            // Arrange
            var client = HttpClientHelpers.CreateClient();
            client.BaseAddress = _fixture.RazorPagesApplication.Url;

            // Arrange & Act
            var result = await client.GetAsync(new Uri("User", UriKind.Relative));

            // Assert
            Assert.Equal(
                _fixture.IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/'),
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);

            Assert.Matches(
                new Regex(
                    "^" +
                    await File.ReadAllTextAsync("Documents/AccountLoginHtml.txt") +
                    "$"
                ),
                await result.Content.ReadAsStringAsync()
            );
        }
    }
}