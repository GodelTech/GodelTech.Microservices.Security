using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Utils;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public class ApiWebsiteIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ApiWebApplication _webApp;
        private readonly IdentityProviderApplication _identityProviderApp;
        private readonly TokenService _tokenService;

        public ApiWebsiteIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Config.ApiWebsiteUrl)
            };

            _identityProviderApp = new IdentityProviderApplication();
            _identityProviderApp.Start();

            _webApp = new ApiWebApplication();
            _webApp.Start();

            _tokenService = new TokenService(Config.IdentityProviderUrl);
        }

        [Fact]
        public async Task SecuredEndpointRequested_When_NoJwtTokenProvided_Should_Return401()
        {
            (await _client.GetAsync("fakes")).StatusCode.Should()
                .Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SecuredEndpointRequested_When_JwtTokenProvidedWithProperScopes_ShouldReturn200()
        {
            var token = await _tokenService.GetClientCredentialsTokenAsync("client", "secret", "fake.add");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            (await _client.GetAsync("fakes")).StatusCode.Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SecuredEndpointRequested_When_JwtTokenProvidedWithoutProperScopes_ShouldReturn403()
        {
            var token = await _tokenService.GetClientCredentialsTokenAsync("client", "secret", "fake.unused");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            (await _client.GetAsync("fakes")).StatusCode.Should()
                .Be(HttpStatusCode.Forbidden);
        }

        public void Dispose()
        {
            _webApp.Stop();
            _identityProviderApp.Stop();
        }
    }
}
