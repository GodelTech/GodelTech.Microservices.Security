using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public sealed class UiSecurityInitializerTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IdentityServerApplication _identityServerApplication;
        private readonly MvcWebApplication _mvcWebApplication;

        public UiSecurityInitializerTests()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = MvcWebApplication.Url
            };

            _identityServerApplication = new IdentityServerApplication();
            _identityServerApplication.Start();

            _mvcWebApplication = new MvcWebApplication();
            _mvcWebApplication.Start();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _identityServerApplication.Stop();
            _mvcWebApplication.Stop();
        }

        [Fact]
        public async Task SecuredPageRequested_RedirectsToIdentityServerLoginPage()
        {
            // Arrange
            var configuration = Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies();
            
            var context = BrowsingContext.New(configuration);

            var expectedIdentityServerUrl = IdentityServerApplication.Url.AbsoluteUri.TrimEnd('/');

            // Act
            var document = await context.OpenAsync(MvcWebApplication.Url.AbsoluteUri);

            // Assert
            Assert.Equal(expectedIdentityServerUrl, document.Location.Origin);
            Assert.Equal("/Account/Login", document.Location.PathName);
        }
        
        // NOTE: Due to limitations of AngleSharp full login workflow can't be properly tested due to
        // lack of JS support and missing cookies.
        // Proper testing requires Selenium tests execution.
    }
}