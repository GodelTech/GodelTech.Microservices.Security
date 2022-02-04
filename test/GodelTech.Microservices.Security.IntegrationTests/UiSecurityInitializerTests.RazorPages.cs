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
    public sealed partial class UiSecurityInitializerTests
    {
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