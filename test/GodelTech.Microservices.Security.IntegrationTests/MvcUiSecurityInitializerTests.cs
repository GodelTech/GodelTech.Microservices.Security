using System;
using System.Net.Http;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class MvcUiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;

        public MvcUiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _httpClientHandler = HttpClientHelpers.CreateHttpClientHandler();

            _httpClient = HttpClientHelpers.CreateHttpClient(
                _httpClientHandler,
                _fixture.MvcApplication.Url
            );
        }

        public void Dispose()
        {
            _httpClientHandler.Dispose();
            _httpClient.Dispose();
        }
    }
}