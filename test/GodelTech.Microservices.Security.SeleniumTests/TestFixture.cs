using System;
using GodelTech.Microservices.Security.SeleniumTests.Applications;

namespace GodelTech.Microservices.Security.SeleniumTests
{
    public sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            IdentityServerApplication = new IdentityServerApplication();

            ApiApplication = new ApiApplication();
            MvcApplication = new MvcApplication();
            RazorPagesApplication = new RazorPagesApplication();

            Start();
        }

        public IdentityServerApplication IdentityServerApplication { get; }

        public ApiApplication ApiApplication { get; }

        public MvcApplication MvcApplication { get; }

        public RazorPagesApplication RazorPagesApplication { get; }

        public void Dispose()
        {
            Stop();
        }

        private void Start()
        {
            IdentityServerApplication.Start();

            ApiApplication.Start();
            MvcApplication.Start();
            RazorPagesApplication.Start();
        }

        private void Stop()
        {
            IdentityServerApplication.Stop();

            ApiApplication.Stop();
            MvcApplication.Stop();
            RazorPagesApplication.Stop();
        }
    }
}