using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.Demo.Api.Models.Fake;
using GodelTech.Microservices.Security.IntegrationTests.Applications;
using GodelTech.Microservices.Security.IntegrationTests.Utils;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public sealed class ApiSecurityInitializerTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ApiWebApplication _apiWebApplication;
        private readonly IdentityProviderApplication _identityProviderApp;
        private readonly TokenService _tokenService;

        public ApiSecurityInitializerTests()
        {
            _httpClient = new HttpClient
            {
                BaseAddress =
                    new Uri(WebApplicationsConfiguration.ApiWebApplicationUrl)
            };

            _apiWebApplication = new ApiWebApplication();
            _apiWebApplication.Start();

            _identityProviderApp = new IdentityProviderApplication();
            _identityProviderApp.Start();

            _tokenService = new TokenService(
                WebApplicationsConfiguration.IdentityProviderWebApplicationUrl
            );
        }

        public static IEnumerable<object[]> HttpGetRequestMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    string.Empty,
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("fakes", UriKind.Relative)
                    },
                    HttpStatusCode.OK
                },
                new object[]
                {
                    string.Empty,
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("fakes/1", UriKind.Relative)
                    },
                    HttpStatusCode.OK
                }
            };

        public static IEnumerable<object[]> HttpPostRequestMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    "fake.add",
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("fakes", UriKind.Relative),
                        Content = new StringContent(
                            JsonSerializer.Serialize(new FakePostModel()),
                            Encoding.UTF8,
                            "application/json"
                        )
                    },
                    HttpStatusCode.Created
                }
            };

        public static IEnumerable<object[]> HttpPutRequestMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    "fake.edit",
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri("fakes/1", UriKind.Relative),
                        Content = new StringContent(
                            JsonSerializer.Serialize(
                                new FakePutModel { Id = 1 }),
                            Encoding.UTF8,
                            "application/json"
                        )
                    },
                    HttpStatusCode.NoContent
                }
            };

        public static IEnumerable<object[]> HttpDeleteRequestMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    "fake.delete",
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri("fakes/1", UriKind.Relative)
                    },
                    HttpStatusCode.OK
                }
            };

        [Theory]
        [MemberData(nameof(HttpGetRequestMemberData))]
        [MemberData(nameof(HttpPostRequestMemberData))]
        [MemberData(nameof(HttpPutRequestMemberData))]
        [MemberData(nameof(HttpDeleteRequestMemberData))]
        public async Task SecuredEndpointRequested_WhenNoJwtTokenProvided_ReturnsUnauthorized(string scope, HttpRequestMessage httpRequestMessage, HttpStatusCode expectedResponseCode)
        {
            // Arrange & Act
            var result =
                await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpGetRequestMemberData))]
        [MemberData(nameof(HttpPostRequestMemberData))]
        [MemberData(nameof(HttpPutRequestMemberData))]
        [MemberData(nameof(HttpDeleteRequestMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvidedWithProperScopes_ReturnsCorrectStatusCode(string scope, HttpRequestMessage httpRequestMessage, HttpStatusCode expectedResponseCode)
        {
            // Arrange 
            var token = await _tokenService.GetClientCredentialsTokenAsync("client", "secret", scope);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Act
            var result =
                await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpPostRequestMemberData))]
        [MemberData(nameof(HttpPutRequestMemberData))]
        [MemberData(nameof(HttpDeleteRequestMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvidedWithoutProperScopes_ReturnsForbidden(string scope, HttpRequestMessage httpRequestMessage, HttpStatusCode expectedResponseCode)
        {
            var token = await _tokenService.GetClientCredentialsTokenAsync("client", "secret", "fake.unused");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var result =
                await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        public void Dispose()
        {
            _apiWebApplication.Stop();
            _identityProviderApp.Stop();
            _httpClient.Dispose();
        }
    }
}
