using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public class ApiWebsiteIntegrationTests : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly ApiWebApplication _webApp;
        private readonly IdentityProviderApplication _identityProviderApp;
        private readonly TokenService _tokenService;

        public ApiWebsiteIntegrationTests()
        {
            _identityProviderApp = new IdentityProviderApplication();
            _identityProviderApp.Start();

            _webApp = new ApiWebApplication();
            _webApp.Start();

            _tokenService = new TokenService("http://localhost:7777");
        }

        [Fact]
        public async Task SecuredEndpointRequested_When_NoJwtTokenProvided_Should_Return401()
        {
            (await _client.GetAsync("http://localhost:5000/weatherforecast")).StatusCode.Should()
                .Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SecuredEndpointRequested_When_JwtTokenProvidedWithProperScopes_ShouldReturn200()
        {
            var token = await _tokenService.GetClientCredentialsTokenAsync("client", "secret", "api1");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            (await _client.GetAsync("http://localhost:5000/weatherforecast")).StatusCode.Should()
                .Be(HttpStatusCode.OK);
        }

        public void Dispose()
        {
            _webApp.Stop();
            _identityProviderApp.Stop();
        }
    }
}
