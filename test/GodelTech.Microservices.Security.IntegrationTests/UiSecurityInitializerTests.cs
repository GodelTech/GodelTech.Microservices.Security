using System;
using System.IO;
using System.Linq;
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
            var expectedIdentityServerUrl = IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/');

            using var client = new HttpClient();

            var expectedResultValue = await File.ReadAllTextAsync("Documents/loginPageHtml.txt");

            // Act
            var result = await client.GetAsync(
                new Uri(RazorPagesApplication.Url, "User")
            );

            var actualHtmlDocument = await result.Content.ReadAsStringAsync();
            actualHtmlDocument = Regex.Replace(actualHtmlDocument, "code_challenge=(.*?);", "code_challenge=(.*?);");
            actualHtmlDocument = Regex.Replace(actualHtmlDocument, "nonce=(.*?);", "nonce=(.*?);");
            actualHtmlDocument = Regex.Replace(actualHtmlDocument, "state=(.*?);", "state=(.*?);");
            actualHtmlDocument = Regex.Replace(actualHtmlDocument, "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*?)\" />", "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*?)\" />");

            // Assert
            Assert.Equal(
                expectedIdentityServerUrl,
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);
            
            Assert.Matches(new Regex(actualHtmlDocument), expectedResultValue);
            /*Assert.Equal(expectedResultValue,
                await result.Content.ReadAsStringAsync()
            );*/
        }
    }
}