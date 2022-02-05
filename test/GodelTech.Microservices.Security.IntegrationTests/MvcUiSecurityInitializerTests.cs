using System;
using System.Net.Http;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: write tests for Mvc demo project (maybe in separate class?)
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
                null // todo: solve baseAddress
            );
        }

        public void Dispose()
        {
            _httpClientHandler.Dispose();
            _httpClient.Dispose();
        }
    }
}