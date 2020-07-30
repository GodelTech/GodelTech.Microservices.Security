using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
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
                BaseAddress = new Uri(Config.ApiWebsiteUrl)
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
            
            var document = await context.OpenAsync(Config.UiWebsiteUrl);

            document.Location.Origin.Should().Be(Config.IdentityProviderUrl);
            document.Location.PathName.Should().Be("/Account/Login");
        }

        
        [Fact]
        public async Task SecuredPageRequested_When_UserLogsIn_Should_RedirectToUserToRequestedPage()
        {
            var config = Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies();
            
            var context = BrowsingContext.New(config);
            
            var document = await context.OpenAsync(Config.UiWebsiteUrl);

            document.Location.PathName.Should().Be("/Account/Login");

            
            var redirectPageContent = await document.Forms[0].SubmitAsync(new
            {
                Username = "bob",
                Password = "bob"
            });

            var sitePageContent = await redirectPageContent.Forms[0].SubmitAsync();
            
            //document.Location.Origin.Should().Be(Config.IdentityProviderUrl);
            
            
            //var result = sitePageContent.DocumentElement.OuterHtml;

            //var form = document.QuerySelector<IHtmlFormElement>("form");
            //var resultDocument = await form.SubmitAsync(new { q = "anglesharp" });            
        }        
        
        public void Dispose()
        {
            _webApp.Stop();
            _identityProviderApp.Stop();
        }
    }
}