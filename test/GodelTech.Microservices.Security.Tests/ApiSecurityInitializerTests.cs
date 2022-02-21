using Moq;
using System;
using System.Collections.Generic;
using FluentAssertions;
using GodelTech.Microservices.Security.ApiSecurity;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.Tests.Fakes.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class ApiSecurityInitializerTests
    {
        private readonly Mock<IAuthorizationPolicyFactory> _mockPolicyFactory;

        private readonly FakeApiSecurityInitializer _initializer;

        public ApiSecurityInitializerTests()
        {
            _mockPolicyFactory = new Mock<IAuthorizationPolicyFactory>(MockBehavior.Strict);

            _initializer = new FakeApiSecurityInitializer(
                x => { },
                _mockPolicyFactory.Object,
                x => { }
            );
        }

        [Fact]
        public void Constructor_WhenApiSecurityOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => new ApiSecurityInitializer(null, _mockPolicyFactory.Object)
            );

            Assert.Equal("configureApiSecurity", result.ParamName);
        }

        [Fact]
        public void Constructor_WhenAuthorizationPolicyFactoryIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () =>
                    new ApiSecurityInitializer(
                        x => { },
                        null,
                        null
                    )
            );

            Assert.Equal("policyFactory", result.ParamName);
        }

        [Fact]
        public void ConfigureJwtBearerOptions_WhenJwtBearerOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureJwtBearerOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureJwtBearerOptions_Success()
        {
            // Arrange
            Action<ApiSecurityOptions> configureApiSecurity = options =>
            {
                options.RequireHttpsMetadata = true;
                options.Authority = "Test Authority";
                options.Issuer = "Test Issuer";
                options.Audience = "Test Audience";
                options.TokenValidation = new TokenValidationOptions
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
            };

            var initializer = new FakeApiSecurityInitializer(
                configureApiSecurity,
                null
            );

            var jwtBearerOptions = new JwtBearerOptions();

            var expectedResult = new JwtBearerOptions
            {
                RequireHttpsMetadata = true,
                Authority = "Test Authority",
                Audience = "Test Audience",
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = "Test Audience",
                    ValidIssuer = "Test Issuer"
                },
                SaveToken = true,
                IncludeErrorDetails = true
            };

            // Act
            initializer.ExposedConfigureJwtBearerOptions(jwtBearerOptions);

            // Assert
            jwtBearerOptions.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void ConfigureAuthorizationOptions_WhenAuthorizationOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureAuthorizationOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureAuthorizationOptions_Success()
        {
            // Arrange
            var policies = new Dictionary<string, AuthorizationPolicy>
            {
                {
                    "fakeKey", AuthorizationPolicyHelpers.GetAuthorizationPolicy("fake.AuthorizationPolicy")
                }
            };
            
            _mockPolicyFactory
                .Setup(x => x.Create())
                .Returns(policies);

            var authorizationOptions = new AuthorizationOptions();

            var expectedResult = new AuthorizationOptions();

            // Act
            _initializer.ExposedConfigureAuthorizationOptions(authorizationOptions);

            // Assert
            authorizationOptions.Should().BeEquivalentTo(expectedResult);

            foreach (var expectedPolicy in policies)
            {
                var actualPolicy = authorizationOptions.GetPolicy(expectedPolicy.Key);
                Assert.Equal(expectedPolicy.Value, actualPolicy);
            }
        }
    }
}