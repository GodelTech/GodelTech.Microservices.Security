using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public partial class ApiSecurityInitializerTests
    {
        public static IEnumerable<object[]> SuccessRequestOpenFakeMemberData =>
            new Collection<object[]>
            {
                // get list
                new object[]
                {
                    OpenFakeHttpRequestMessages[0],
                    HttpStatusCode.OK
                }
            };

        [Theory]
        [MemberData(nameof(SuccessRequestOpenFakeMemberData))]
        public async Task UnsecuredEndpointRequested_WhenJwtTokenNotProvided_ReturnsCorrectStatusCode(
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange & Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }

        public static IEnumerable<object[]> AuthorizedRequestOpenFakeMemberData =>
            new Collection<object[]>
            {
                // get list
                new object[]
                {
                    "api",
                    OpenFakeHttpRequestMessages[0],
                    HttpStatusCode.OK
                }
            };

        [Theory]
        [MemberData(nameof(AuthorizedRequestOpenFakeMemberData))]
        public async Task UnsecuredEndpointRequested_WhenJwtTokenProvided_ReturnsCorrectStatusCode(
            string scope,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange 
            var token = await _fixture.TokenService.GetClientCredentialsTokenAsync(
                "ClientForApi",
                "secret",
                scope
            );

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }
    }
}
