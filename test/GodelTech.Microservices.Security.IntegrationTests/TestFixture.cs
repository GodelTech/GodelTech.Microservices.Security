using System;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Utils;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public sealed class TestFixture : IDisposable
    {
        private readonly IdentityServerApplication _identityServerApplication;

        private readonly ApiApplication _apiApplication;
        private readonly RazorPagesApplication _razorPagesApplication;

        public TestFixture()
        {
            _identityServerApplication = new IdentityServerApplication();
            _identityServerApplication.Start();

            TokenService = new TokenService(IdentityServerApplication.Url);

            _apiApplication = new ApiApplication();
            _apiApplication.Start();

            _razorPagesApplication = new RazorPagesApplication();
            _razorPagesApplication.Start();
        }

        public TokenService TokenService { get; }

        public void Dispose()
        {
            _identityServerApplication.Stop();

            _apiApplication.Stop();
            _razorPagesApplication.Stop();
        }
    }
}