using System;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.SeleniumTests.Applications;
using Xunit;

namespace GodelTech.Microservices.Security.SeleniumTests
{
    public sealed class TestFixture : IDisposable, IAsyncLifetime
    {
        public TestFixture()
        {
            IdentityServerApplication = new IdentityServerApplication();

            ApiApplication = new ApiApplication();
            MvcApplication = new MvcApplication();
            RazorPagesApplication = new RazorPagesApplication();
        }

        public IdentityServerApplication IdentityServerApplication { get; }

        public ApiApplication ApiApplication { get; }

        public MvcApplication MvcApplication { get; }

        public RazorPagesApplication RazorPagesApplication { get; }

        public async Task InitializeAsync()
        {
            await IdentityServerApplication.StartAsync();

            await ApiApplication.StartAsync();
            await MvcApplication.StartAsync();
            await RazorPagesApplication.StartAsync();
        }

        public Task DisposeAsync()
        {
            return Task.WhenAll(
                IdentityServerApplication.StopAsync(),

                ApiApplication.StopAsync(),
                MvcApplication.StopAsync(),
                RazorPagesApplication.StopAsync()
            );
        }

        public void Dispose()
        {
            IdentityServerApplication.Dispose();

            ApiApplication.Dispose();
            MvcApplication.Dispose();
            RazorPagesApplication.Dispose();
        }
    }
}
