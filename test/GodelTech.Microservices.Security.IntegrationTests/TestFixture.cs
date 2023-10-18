using System;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.SeleniumTests.Applications;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
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

        public ServiceProvider ServiceProvider { get; private set; }


        public async Task InitializeAsync()
        {
            await IdentityServerApplication.StartAsync();

            await ApiApplication.StartAsync();
            await MvcApplication.StartAsync();
            await RazorPagesApplication.StartAsync();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
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

        private void ConfigureServices(IServiceCollection services)
        {
            AddHttpClient(services, "ApiClient", ApiApplication.Url);
            AddHttpClient(services, "MvcClient", MvcApplication.Url);
            AddHttpClient(services, "MvcSecondClient", MvcApplication.Url);
            AddHttpClient(services, "RazorPagesClient", RazorPagesApplication.Url);
            AddHttpClient(services, "RazorPagesSecondClient", RazorPagesApplication.Url);
        }

        private static void AddHttpClient(IServiceCollection services, string name, Uri baseAddress)
        {
            services
                .AddHttpClient(
                    name,
                    client =>
                    {
                        client.BaseAddress = baseAddress;
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                );
        }
    }
}
