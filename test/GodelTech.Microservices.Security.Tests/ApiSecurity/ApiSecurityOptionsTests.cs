using FluentAssertions;
using GodelTech.Microservices.Security.ApiSecurity;
using Xunit;

namespace GodelTech.Microservices.Security.Tests.ApiSecurity
{
    public class ApiSecurityOptionsTests
    {
        private readonly ApiSecurityOptions _options;

        public ApiSecurityOptionsTests()
        {
            _options = new ApiSecurityOptions();
        }

        [Fact]
        public void RequireHttpsMetadata_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.RequireHttpsMetadata);
        }

        [Fact]
        public void RequireHttpsMetadata_Set_Success()
        {
            // Arrange

            // Act
            _options.RequireHttpsMetadata = false;

            // Assert
            Assert.False(_options.RequireHttpsMetadata);
        }

        [Fact]
        public void Authority_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.Authority);
        }

        [Fact]
        public void Authority_Set_Success()
        {
            // Arrange
            var expectedResult = "Test XmlCommentsFilePath";

            // Act
            _options.Authority = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.Authority);
        }

        [Fact]
        public void Issuer_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.Issuer);
        }

        [Fact]
        public void Issuer_Set_Success()
        {
            // Arrange
            var expectedResult = "Test Issuer";

            // Act
            _options.Issuer = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.Issuer);
        }

        [Fact]
        public void Audience_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.Audience);
        }

        [Fact]
        public void Audience_Set_Success()
        {
            // Arrange
            var expectedResult = "Test Audience";

            // Act
            _options.Audience = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.Audience);
        }

        [Fact]
        public void TokenValidation_Get_Success()
        {
            // Arrange
            var expectedResult = new TokenValidationOptions();

            // Act & Assert
            expectedResult.Should().BeEquivalentTo(_options.TokenValidation);
        }

        [Fact]
        public void TokenValidation_Set_Success()
        {
            // Arrange
            var expectedResult = new TokenValidationOptions();

            // Act
            _options.TokenValidation = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.TokenValidation);
        }

        [Fact]
        public void SaveToken_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.SaveToken);
        }

        [Fact]
        public void SaveToken_Set_Success()
        {
            // Arrange & Act
            _options.SaveToken = false;

            // Assert
            Assert.False(_options.SaveToken);
        }

        [Fact]
        public void IncludeErrorDetails_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.IncludeErrorDetails);
        }

        [Fact]
        public void IncludeErrorDetails_Set_Success()
        {
            // Arrange & Act
            _options.IncludeErrorDetails = false;

            // Assert
            Assert.False(_options.IncludeErrorDetails);
        }
    }
}
