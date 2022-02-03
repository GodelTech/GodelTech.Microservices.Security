﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using IdentityServer.Quickstart.Account;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
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

            // Act
            var result = await client.GetAsync(new Uri(RazorPagesApplication.Url.AbsoluteUri));

            var content = await result.Content.ReadAsStringAsync();

            var file = AppDomain.CurrentDomain.BaseDirectory;


            // Assert
            Assert.Equal(
                expectedIdentityServerUrl,
                result.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority)
            );
            Assert.Equal("/Account/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }
        
        // NOTE: Due to limitations of AngleSharp full login workflow can't be properly tested due to
        // lack of JS support and missing cookies.
        // Proper testing requires Selenium tests execution.
    }
}