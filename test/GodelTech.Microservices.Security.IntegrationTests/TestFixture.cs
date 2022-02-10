using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.SeleniumTests.Applications;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        public async Task AuthorizeClientAsync(HttpClient httpClient, string scope)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(IdentityServerApplication.Url.AbsoluteUri);
            if (discoveryDocument.IsError) throw new InvalidOperationException(discoveryDocument.Error);

            using var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "ClientForApi",
                ClientSecret = "secret",
                Scope = scope
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
            if (tokenResponse.IsError) throw new InvalidOperationException(tokenResponse.Error);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResponse.AccessToken);
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
            services
                .AddHttpClient(
                    "ApiClient",
                    client =>
                    {
                        client.BaseAddress = ApiApplication.Url;
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                );

            services
                .AddHttpClient(
                    "RazorPagesClient",
                    client =>
                    {
                        client.BaseAddress = RazorPagesApplication.Url;
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