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
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: write tests for openFake method without security
    // todo: add UseHsts for demo projects???
    [Collection("TestCollection")]
    public sealed class ApiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClient _httpClient;

        public ApiSecurityInitializerTests(TestFixture fixture)
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
        public async Task SecuredEndpointRequested_WhenNoJwtTokenProvided_ReturnsUnauthorized(
            string scope,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange & Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpGetRequestMemberData))]
        [MemberData(nameof(HttpPostRequestMemberData))]
        [MemberData(nameof(HttpPutRequestMemberData))]
        [MemberData(nameof(HttpDeleteRequestMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvidedWithProperScopes_ReturnsCorrectStatusCode(
            string scope,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange 
            var token = await _fixture.TokenService.GetClientCredentialsTokenAsync("ClientForApi", "secret", scope);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpPostRequestMemberData))]
        [MemberData(nameof(HttpPutRequestMemberData))]
        [MemberData(nameof(HttpDeleteRequestMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvidedWithoutProperScopes_ReturnsForbidden(
            string scope,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange
            var token = await _fixture.TokenService.GetClientCredentialsTokenAsync(
                "ClientForApi",
                "secret",
                "fake.unused"
            );

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }
    }
}
