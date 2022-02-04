using System;
using System.Net.Http;
using GodelTech.Microservices.Security.IntegrationTests.Applications;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: add UseHsts for demo projects???
    public sealed partial class UiSecurityInitializerTests : IDisposable
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
    }
}