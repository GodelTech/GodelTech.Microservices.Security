using System;
using System.Collections.Generic;
using FluentAssertions;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.UiSecurity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class UiSecurityInitializerTests
    {
        private FakeUiSecurityInitializer Initializer { get; set; }

        public UiSecurityInitializerTests()
        {
            Initializer = new FakeUiSecurityInitializer(
                x => { }
            );
        }

        [Fact]
        public void Constructor_WhenUiSecurityOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => new FakeUiSecurityInitializer(null)
            );

            Assert.Equal("configureUiSecurity", result.ParamName);
        }

        [Fact]
        public void ConfigureOpenIdConnectOptions_WhenOpenIdConnectOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () =>
                    Initializer.ExposedConfigureOpenIdConnectOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureOpenIdConnectOptions_Success()
        {
            // Arrange
            var uiSecurityOptions = new UiSecurityOptions
            {
                Authority = "Test Authority",
                ClientId = "Test ClientId",
                ClientSecret = "Test ClientSecret",
                GetClaimsFromUserInfoEndpoint = true,
                RequireHttpsMetadata = true,
                ResponseType = "Test ResponseType",
                Issuer = "Test Issuer",
                UsePkce = true,
                SaveTokens = true,
                Scopes = new List<string>
                {
                    "Test Scope1",
                    "Test Scope2"
                }
            };

            var openIdConnectOptions = new OpenIdConnectOptions();

            // Act
            Initializer = new FakeUiSecurityInitializer(
                x =>
                {
                    x.Authority = "Test Authority";
                    x.ClientId = "Test ClientId";
                    x.GetClaimsFromUserInfoEndpoint = true;
                    x.RequireHttpsMetadata = true;
                    x.ResponseType = "Test ResponseType";
                    x.Issuer = "Test Issuer";
                    x.UsePkce = true;
                    x.SaveTokens = true;
                    x.Scopes = new List<string>
                    {
                        "Test Scope1",
                        "Test Scope2"
                    };
                    x.ClientSecret = "Test ClientSecret";
                }
            );

            Initializer.ExposedConfigureOpenIdConnectOptions(openIdConnectOptions);

            // Assert
            Assert.Equal(openIdConnectOptions.Authority, uiSecurityOptions.Authority);
            Assert.Equal(openIdConnectOptions.ClientId, uiSecurityOptions.ClientId);
            Assert.Equal(openIdConnectOptions.ClientSecret, uiSecurityOptions.ClientSecret);
            Assert.Equal(openIdConnectOptions.GetClaimsFromUserInfoEndpoint, uiSecurityOptions.GetClaimsFromUserInfoEndpoint);
            Assert.Equal(openIdConnectOptions.RequireHttpsMetadata, uiSecurityOptions.RequireHttpsMetadata);
            Assert.Equal(openIdConnectOptions.ResponseType, uiSecurityOptions.ResponseType);
            Assert.Equal(openIdConnectOptions.TokenValidationParameters.ValidIssuer, uiSecurityOptions.Issuer);
            Assert.Equal(openIdConnectOptions.UsePkce, uiSecurityOptions.UsePkce);
            Assert.Equal(openIdConnectOptions.SaveTokens, uiSecurityOptions.SaveTokens);
            openIdConnectOptions.Scope.Should().BeEquivalentTo(uiSecurityOptions.Scopes);
        }
    }
}