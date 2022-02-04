using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: write tests for User page (with login)
    // todo: write tests for Mvc demo project (maybe in separate class?)
    // todo: add UseHsts for demo projects???
    public sealed class UiSecurityInitializerTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IdentityServerApplication _identityServerApplication;
        private readonly RazorPagesApplication _razorPagesApplication;

        public UiSecurityInitializerTests()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = RazorPagesApplication.Url
            };

            _identityServerApplication = new IdentityServerApplication();
            _identityServerApplication.Start();

            _razorPagesApplication = new RazorPagesApplication();
            _razorPagesApplication.Start();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _identityServerApplication.Stop();
            _razorPagesApplication.Stop();
        }

        [Fact]
        public async Task SecuredPageRequested_RedirectsToIdentityServerLoginPage()
        {
            // Arrange
            using var client = new HttpClient();

            // Act
            var result = await client.GetAsync(
                new Uri(RazorPagesApplication.Url, "User")
            );

            // Assert
            Assert.Equal(
                IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/'),
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