using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class MvcUiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClient _httpClient;

        public MvcUiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            var httpClientFactory = _fixture.ServiceProvider.GetService<IHttpClientFactory>();

            _httpClient = httpClientFactory.CreateClient("MvcClient");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}