using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using GodelTech.Microservices.Security.UiSecurity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit;

namespace GodelTech.Microservices.Security.Tests.UiSecurity
{
    public class UiSecurityOptionsTests
    {
        private readonly UiSecurityOptions _options;

        public UiSecurityOptionsTests()
        {
            _options = new UiSecurityOptions();
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
            var expectedResult = "Test Authority";

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
        public void ClientId_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.ClientId);
        }

        [Fact]
        public void ClientId_Set_Success()
        {
            // Arrange
            var expectedResult = "Test ClientId";

            // Act
            _options.ClientId = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.ClientId);
        }

        [Fact]
        public void ClientSecret_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.ClientSecret);
        }

        [Fact]
        public void ClientSecret_Set_Success()
        {
            // Arrange
            var expectedResult = "Test ClientSecret";

            // Act
            _options.ClientSecret = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.ClientSecret);
        }

        [Fact]
        public void GetClaimsFromUserInfoEndpoint_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.GetClaimsFromUserInfoEndpoint);
        }

        [Fact]
        public void GetClaimsFromUserInfoEndpoint_Set_Success()
        {
            // Arrange & Act
            _options.GetClaimsFromUserInfoEndpoint = false;

            // Assert
            Assert.False(_options.GetClaimsFromUserInfoEndpoint);
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
            // Arrange & Act
            _options.RequireHttpsMetadata = false;

            // Assert
            Assert.False(_options.RequireHttpsMetadata);
        }

        [Fact]
        public void ResponseType_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Equal(OpenIdConnectResponseType.Code, _options.ResponseType);
        }

        [Fact]
        public void ResponseType_Set_Success()
        {
            // Arrange
            var expectedResult = "Test ResponseType";

            // Act
            _options.ResponseType = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.ResponseType);
        }

        [Fact]
        public void Scopes_Get_Success()
        {
            // Arrange & Act & Assert
            _options.Scopes.Should().BeEquivalentTo(new Collection<string>());
        }

        [Fact]
        public void Scopes_Set_Success()
        {
            // Arrange
            var expectedResult = new Collection<string>
            {
                "Test Scope"
            };

            // Act
            _options.Scopes = expectedResult;

            // Act
            _options.Scopes.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void UsePkce_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.UsePkce);
        }

        [Fact]
        public void UsePkce_Set_Success()
        {
            // Arrange & Act
            _options.UsePkce = false;

            // Assert
            Assert.False(_options.UsePkce);
        }

        [Fact]
        public void PublicAuthorityUri_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Null(_options.PublicAuthorityUri);
        }

        [Fact]
        public void PublicAuthorityUri_Set_Success()
        {
            // Arrange
            var expectedResult = new Uri("https://test.dev");

            // Act
            _options.PublicAuthorityUri = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.PublicAuthorityUri);
        }

        [Fact]
        public void SaveTokens_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.SaveTokens);
        }

        [Fact]
        public void SaveTokens_Set_Success()
        {
            // Arrange & Act
            _options.SaveTokens = false;

            // Assert
            Assert.False(_options.SaveTokens);
        }
    }
}