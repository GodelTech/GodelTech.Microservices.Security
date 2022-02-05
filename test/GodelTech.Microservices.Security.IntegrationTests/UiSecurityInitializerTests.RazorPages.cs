using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: write tests for User page (with login)
    public sealed partial class UiSecurityInitializerTests
    {
        [Fact]
        public async Task SecuredPageRequested_RedirectsToIdentityServerLoginPage()
        {
            // Arrange & Act
            var result = await _httpClient.GetAsync(
                new Uri(_fixture.RazorPagesApplication.Url, "User")
            );

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