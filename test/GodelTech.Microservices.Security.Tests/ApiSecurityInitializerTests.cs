using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.Microservices.Security.ApiSecurity;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.Tests.Fakes.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
                x =>
                    new ApiSecurityOptions(),
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
                        x =>
                            new ApiSecurityOptions(),
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
            _initializer.ExposedConfigureAuthorizationOptions(options);

            // Assert
            var actualPolicy = options.GetPolicy("fakeKey");
            Assert.NotNull(actualPolicy);

            var actualAuthorizationRequirement = actualPolicy.Requirements[1] as ClaimsAuthorizationRequirement;
            var allowedValue =
                actualAuthorizationRequirement?.AllowedValues.Single(s => s == "fake.AuthorizationPolicy");

            Assert.Equal("fake.AuthorizationPolicy", allowedValue);
        }
    }
}