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
        public static IEnumerable<object[]> UnauthorizedFakeMemberData
        {
            get
            {
                // get list
                yield return new object[] {FakeHttpRequestMessages[0]};
                // get
                yield return new object[] {FakeHttpRequestMessages[1]};
                // post
                yield return new object[] {FakeHttpRequestMessages[2]};
                // put
                yield return new object[] {FakeHttpRequestMessages[3]};
                // delete
                yield return new object[] {FakeHttpRequestMessages[4]};
            }
        }

        [Theory]
        [MemberData(nameof(UnauthorizedFakeMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenNotProvided_ReturnsUnauthorized(HttpRequestMessage httpRequestMessage)
        {
            // Arrange & Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        public static IEnumerable<object[]> SuccessRequestFakeMemberData =>
            new Collection<object[]>
            {
                // get list
                new object[]
                {
                    "api",
                    FakeHttpRequestMessages[0],
                    HttpStatusCode.OK
                },
                // get
                new object[]
                {
                    "api",
                    FakeHttpRequestMessages[1],
                    HttpStatusCode.OK
                },
                // post
                new object[]
                {
                    "fake.add",
                    FakeHttpRequestMessages[2],
                    HttpStatusCode.Created
                },
                // put
                new object[]
                {
                    "fake.edit",
                    FakeHttpRequestMessages[3],
                    HttpStatusCode.NoContent
                },
                // delete
                new object[]
                {
                    "fake.delete",
                    FakeHttpRequestMessages[4],
                    HttpStatusCode.OK
                },
            };

        [Theory]
        [MemberData(nameof(SuccessRequestFakeMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvided_ReturnsCorrectStatusCode(
            string scope,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode expectedResponseCode)
        {
            // Arrange
            await AuthorizeClientAsync(_httpClient, scope);

            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(expectedResponseCode, result.StatusCode);
        }

        public static IEnumerable<object[]> ForbiddenFakeMemberData
        {
            get
            {
                // post
                yield return new object[] {FakeHttpRequestMessages[2]};
                // put
                yield return new object[] {FakeHttpRequestMessages[3]};
                // delete
                yield return new object[] {FakeHttpRequestMessages[4]};
            }
        }

        [Theory]
        [MemberData(nameof(ForbiddenFakeMemberData))]
        public async Task SecuredEndpointRequested_WhenJwtTokenProvidedWithoutProperScope_ReturnsForbidden(HttpRequestMessage httpRequestMessage)
        {
            // Arrange
            await AuthorizeClientAsync(_httpClient, "fake.unused");

            // Act
            var result = await _httpClient.SendAsync(httpRequestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }
    }
}
