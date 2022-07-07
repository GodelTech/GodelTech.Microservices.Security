using GodelTech.Microservices.Security.ApiSecurity;
using Xunit;

namespace GodelTech.Microservices.Security.Tests.ApiSecurity
{
    public class TokenValidationOptionsTests
    {
        private readonly TokenValidationOptions _options;

        public TokenValidationOptionsTests()
        {
            _options = new TokenValidationOptions();
        }

        [Fact]
        public void ValidateAudience_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ValidateAudience);
        }

        [Fact]
        public void ValidateAudience_Set_Success()
        {
            // Arrange & Act
            _options.ValidateAudience = false;

            // Assert
            Assert.False(_options.ValidateAudience);
        }

        [Fact]
        public void ValidateIssuer_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ValidateIssuer);
        }

        [Fact]
        public void ValidateIssuer_Set_Success()
        {
            // Arrange & Act
            _options.ValidateIssuer = false;

            // Assert
            Assert.False(_options.ValidateIssuer);
        }

        [Fact]
        public void ValidateIssuerSigningKey_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ValidateIssuerSigningKey);
        }

        [Fact]
        public void ValidateIssuerSigningKey_Set_Success()
        {
            // Arrange & Act
            _options.ValidateIssuerSigningKey = false;

            // Assert
            Assert.False(_options.ValidateIssuerSigningKey);
        }

        [Fact]
        public void ValidateLifetime_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ValidateLifetime);
        }

        [Fact]
        public void ValidateLifetime_Set_Success()
        {
            // Arrange & Act
            _options.ValidateLifetime = false;

            // Assert
            Assert.False(_options.ValidateLifetime);
        }

    }
}
