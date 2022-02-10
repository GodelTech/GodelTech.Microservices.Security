using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
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
            await _fixture.AuthorizeClientAsync(_httpClient, scope);

            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }
    }
}
