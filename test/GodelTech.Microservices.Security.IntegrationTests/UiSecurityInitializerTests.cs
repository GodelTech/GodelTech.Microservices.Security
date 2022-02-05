using System;
using System.Net.Http;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: add UseHsts for demo projects???
    [Collection("TestCollection")]
    public sealed partial class UiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClient _httpClient;

        public UiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _httpClient = new HttpClient
            {
                BaseAddress = _fixture.ApiApplication.Url
            };
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}