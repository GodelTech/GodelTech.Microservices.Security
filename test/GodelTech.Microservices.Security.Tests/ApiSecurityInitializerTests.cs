using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.Microservices.Security.ApiSecurity;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.Tests.Fakes.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class ApiSecurityInitializerTests
    {
        private readonly Mock<IAuthorizationPolicyFactory> _mockPolicyFactory;

        private FakeApiSecurityInitializer Initializer { get; set; }

        public ApiSecurityInitializerTests()
        {
            _mockPolicyFactory = new Mock<IAuthorizationPolicyFactory>(MockBehavior.Strict);

            Initializer = new FakeApiSecurityInitializer(
                x => { },
                _mockPolicyFactory.Object
            );
        }

        [Fact]
        public void Constructor_WhenApiSecurityOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange& Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => new ApiSecurityInitializer(null, _mockPolicyFactory.Object));

            Assert.Equal("configureApiSecurity", result.ParamName);
        }

        [Fact]
        public void Constructor_WhenAuthorizationPolicyFactoryIsNull_ThrowsArgumentNullException()
        {
            // Arrange& Act & Assert
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
                () => Initializer.ExposedConfigureJwtBearerOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureAuthorizationOptions_WhenAuthorizationOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => Initializer.ExposedConfigureAuthorizationOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureAuthorizationOptions_Success()
        {
            // Arrange
            var options = new AuthorizationOptions();
            var policies = new Dictionary<string, AuthorizationPolicy>
            {
                {
                    "fakeKey", AuthorizationPolicyExtensions.GetAuthorizationPolicy("fake.AuthorizationPolicy")
                }
            };
            
            _mockPolicyFactory.Setup(x => x.Create())
                .Returns(policies);

            // Act
            Initializer.ExposedConfigureAuthorizationOptions(options);

            // Assert
            var actualPolicy = options.GetPolicy("fakeKey");
            Assert.NotNull(actualPolicy);

            var actualAuthorizationRequirement = actualPolicy.Requirements[1] as ClaimsAuthorizationRequirement;
            var allowedValue =
                actualAuthorizationRequirement?.AllowedValues.Single(s => s == "fake.AuthorizationPolicy");

            Assert.Equal("fake.AuthorizationPolicy", allowedValue);
        }

        [Fact]
        public void ConfigureJwtBearerOptions_Success()
        {
            // Arrange
            var apiSecurityOptions = new ApiSecurityOptions
            {
                RequireHttpsMetadata = true,
                Authority = "Test Authority",
                Audience = "Test Audience",
                TokenValidation = new TokenValidationOptions
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                },
                Issuer = "Test Issuer",
                SaveToken = true,
                IncludeErrorDetails = true
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            Initializer = new FakeApiSecurityInitializer(
                x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.Authority = "Test Authority";
                    x.Audience = "Test Audience";
                    x.TokenValidation.ValidateAudience = true;
                    x.TokenValidation.ValidateIssuer = true;
                    x.TokenValidation.ValidateIssuerSigningKey = true;
                    x.TokenValidation.ValidateLifetime = true;
                    x.Issuer = "Test Issuer";
                    x.SaveToken = true;
                    x.IncludeErrorDetails = true;
                }
            );

            Initializer.ExposedConfigureJwtBearerOptions(jwtBearerOptions);

            // Assert
            Assert.Equal(jwtBearerOptions.RequireHttpsMetadata, apiSecurityOptions.RequireHttpsMetadata);
            Assert.Equal(jwtBearerOptions.Authority, apiSecurityOptions.Authority);
            Assert.Equal(jwtBearerOptions.Audience, apiSecurityOptions.Audience);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidateAudience, apiSecurityOptions.TokenValidation.ValidateAudience);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidateIssuer, apiSecurityOptions.TokenValidation.ValidateIssuer);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey, apiSecurityOptions.TokenValidation.ValidateIssuerSigningKey);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidateLifetime, apiSecurityOptions.TokenValidation.ValidateLifetime);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidAudience, apiSecurityOptions.Audience);
            Assert.Equal(jwtBearerOptions.TokenValidationParameters.ValidIssuer, apiSecurityOptions.Issuer);
            Assert.Equal(jwtBearerOptions.SaveToken, apiSecurityOptions.SaveToken);
            Assert.Equal(jwtBearerOptions.IncludeErrorDetails, apiSecurityOptions.IncludeErrorDetails);
        }
    }
}