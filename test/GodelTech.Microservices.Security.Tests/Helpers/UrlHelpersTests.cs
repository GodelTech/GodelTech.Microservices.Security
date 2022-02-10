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

        [Fact]
        public void ChangeAuthority_WhenPublicAuthorityUri_Success()
        {
            // Arrange
            var publicAuthorityUri = new Uri("https://test.publicauthority");

            // Act
            var result = UrlHelpers.ChangeAuthority("https://test.authority/testAbsoluteUri", publicAuthorityUri);

            // Assert
            Assert.Equal("https://test.publicauthority/testAbsoluteUri", result);
        }
    }
}