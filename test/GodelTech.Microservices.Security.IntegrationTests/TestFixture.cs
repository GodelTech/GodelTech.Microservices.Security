using System;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Utils;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            IdentityServerApplication = new IdentityServerApplication();

            ApiApplication = new ApiApplication();
            RazorPagesApplication = new RazorPagesApplication();

            TokenService = new TokenService(IdentityServerApplication.Url);

            Start();
        }

        public IdentityServerApplication IdentityServerApplication { get; }

        public ApiApplication ApiApplication { get; }

        public RazorPagesApplication RazorPagesApplication { get; }

        public TokenService TokenService { get; }

        public void Dispose()
        {
            Stop();
        }

        private void Start()
        {
            IdentityServerApplication.Start();

            ApiApplication.Start();
            RazorPagesApplication.Start();
        }

        private void Stop()
        {
            IdentityServerApplication.Stop();

            ApiApplication.Stop();
            RazorPagesApplication.Stop();
        }
    }
}