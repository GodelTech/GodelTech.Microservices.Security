using System;
using GodelTech.Microservices.Security.Helpers;
using Xunit;

namespace GodelTech.Microservices.Security.Tests.Helpers
{
    public class UrlHelpersTests
    {
        [Fact]
        public void ChangeAuthority_WhenPublicAuthorityUriIsNull_Success()
        {
            // Arrange & Act
            var result = UrlHelpers.ChangeAuthority("Test Authority", null);

            // Assert
            Assert.Equal("Test Authority", result);
        }

        [Theory]
        // ends with /
        [InlineData("https://localhost/", "http://localhost:81", "http://localhost:81/")]
        // localhost -> localhost:81
        [InlineData("http://localhost", "http://localhost:81", "http://localhost:81/")]
        [InlineData("http://localhost/fake", "http://localhost:81", "http://localhost:81/fake")]
        [InlineData("http://localhost/fake?a=b", "http://localhost:81", "http://localhost:81/fake?a=b")]
        // localhost -> test.com
        [InlineData("http://localhost", "http://test.com", "http://test.com/")]
        [InlineData("http://localhost/fake", "http://test.com", "http://test.com/fake")]
        [InlineData("http://localhost/fake?a=b", "http://test.com", "http://test.com/fake?a=b")]
        // localhost:80 -> localhost:81
        [InlineData("http://localhost:80", "http://localhost:81", "http://localhost:81/")]
        [InlineData("http://localhost:80/fake", "http://localhost:81", "http://localhost:81/fake")]
        [InlineData("http://localhost:80/fake?a=b", "http://localhost:81", "http://localhost:81/fake?a=b")]
        // example.com -> test.com
        [InlineData("http://example.com", "http://test.com", "http://test.com/")]
        [InlineData("http://example.com/fake", "http://test.com", "http://test.com/fake")]
        [InlineData("http://example.com/fake?a=b", "http://test.com", "http://test.com/fake?a=b")]
        // https
        [InlineData("https://localhost/fake?a=b", "https://localhost:81", "https://localhost:81/fake?a=b")]
        [InlineData("https://localhost/fake?a=b", "https://test.com", "https://test.com/fake?a=b")]
        [InlineData("https://localhost:80/fake?a=b", "https://localhost:81", "https://localhost:81/fake?a=b")]
        [InlineData("https://example.com/fake?a=b", "https://test.com", "https://test.com/fake?a=b")]
        public void ChangeAuthority_Success(string issuerAddress, string publicAuthority, string expectedResult)
        {
            // Arrange
            var publicAuthorityUri = new Uri(publicAuthority);

            // Act
            var result = UrlHelpers.ChangeAuthority(issuerAddress, publicAuthorityUri);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
