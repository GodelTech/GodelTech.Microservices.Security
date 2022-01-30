using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using FluentAssertions;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Utils;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public class UiWebsiteIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly UiWebApplication _webApp;
        private readonly IdentityProviderApplication _identityProviderApp;

        public UiWebsiteIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(WebApplicationsConfiguration.ApiWebApplicationUrl)
            };

            _identityProviderApp = new IdentityProviderApplication();
            _identityProviderApp.Start();

            _webApp = new UiWebApplication();
            _webApp.Start();
        }

        [Fact]
        public async Task SecuredPageRequested_Should_RedirectToIdentityProviderLoginPage()
        {
            var config = Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies();
            
            var context = BrowsingContext.New(config);
            
            var document = await context.OpenAsync(WebApplicationsConfiguration.MvcWebApplicationUrl);

            document.Location.Origin.Should().Be(WebApplicationsConfiguration.IdentityProviderWebApplicationUrl);
            document.Location.PathName.Should().Be("/Account/Login");
        }
        
        // NOTE: Due to limitations of AngleSharp full login workflow can't be properly tested due to
        // lack of JS support and missing cookies.
        // Proper testing requires Selenium tests execution.

        public void Dispose()
        {
            _webApp.Stop();
            _identityProviderApp.Stop();
        }
    }
}