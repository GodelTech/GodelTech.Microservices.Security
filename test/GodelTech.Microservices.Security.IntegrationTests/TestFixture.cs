using System;
using System.Net.Http;
using GodelTech.Microservices.Security.SeleniumTests.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.IntegrationTests
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

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public IdentityServerApplication IdentityServerApplication { get; }

        public ApiApplication ApiApplication { get; }

        public MvcApplication MvcApplication { get; }

        public RazorPagesApplication RazorPagesApplication { get; }

        public ServiceProvider ServiceProvider { get; }

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