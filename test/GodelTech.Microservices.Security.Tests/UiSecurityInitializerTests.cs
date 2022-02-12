using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.UiSecurity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class UiSecurityInitializerTests
    {
        private readonly FakeUiSecurityInitializer _initializer;

        public UiSecurityInitializerTests()
        {
            _initializer = new FakeUiSecurityInitializer(
                x => { },
                x => { }
            );
        }

        [Fact]
        public void Constructor_WhenUiSecurityOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => new UiSecurityInitializer(null)
            );

            Assert.Equal("configureUiSecurity", result.ParamName);
        }

        [Fact]
        public void ConfigureOpenIdConnectOptions_WhenOpenIdConnectOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var result = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureOpenIdConnectOptions(null)
            );

            Assert.Equal("options", result.ParamName);
        }

        [Fact]
        public void ConfigureOpenIdConnectOptions_Success()
        {
            // Arrange
            Action<UiSecurityOptions> configureUiSecurity = options =>
            {
                options.Authority = "Test Authority";
                options.Issuer = "Test Issuer";
                options.ClientId = "Test ClientId";
                options.ClientSecret = "Test ClientSecret";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = true;
                options.ResponseType = "Test ResponseType";
                options.Scopes = new List<string>
                {
                    "Test Scope1",
                    "Test Scope2"
                };
                options.UsePkce = true;
                options.PublicAuthorityUri = new Uri("https://publicauthority.dev");
                options.SaveTokens = true;
            };

            var initializer = new FakeUiSecurityInitializer(
                configureUiSecurity,
                null
            );

            var openIdConnectOptions = new OpenIdConnectOptions();

            var expectedResult = new OpenIdConnectOptions
            {
                Authority = "Test Authority",
                ClientId = "Test ClientId",
                ClientSecret = "Test ClientSecret",
                GetClaimsFromUserInfoEndpoint = true,
                RequireHttpsMetadata = true,
                ResponseType = "Test ResponseType",
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "Test Issuer"
                },
                UsePkce = true,
                Events = initializer.ExposedCreateOpenIdConnectEvents(),
                SaveTokens = true
            };
            expectedResult.Scope.Clear();
            expectedResult.Scope.Add("Test Scope1");
            expectedResult.Scope.Add("Test Scope2");


            // Act
            initializer.ExposedConfigureOpenIdConnectOptions(openIdConnectOptions);

            // Assert
            openIdConnectOptions.Should().BeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> CreateOpenIdConnectEventsMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new FakeUiSecurityInitializer(
                        x => { },
                        null
                    ),
                    "/Errors/Fault"
                },
                new object[]
                {
                    new FakeUiSecurityInitializer(
                        x => { },
                        x => { },
                        "/TestFailurePath"
                    ),
                    "/TestFailurePath"
                }
            };

        [Theory]
        [MemberData(nameof(CreateOpenIdConnectEventsMemberData))]
        public void CreateOpenIdConnectEvents_WhenPublicAuthorityUriIsNull(
            FakeUiSecurityInitializer initializer,
            string expectedFailurePath)
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(x => x.Response.Redirect(expectedFailurePath));

            var mockRemoteFailureContext = new Mock<RemoteFailureContext>(
                MockBehavior.Strict,
                mockHttpContext.Object,
                new AuthenticationScheme(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme,
                    typeof(AuthenticationHandler<AuthenticationSchemeOptions>)
                ),
                new RemoteAuthenticationOptions(),
                new InvalidOperationException()
            );

            // Act
            var events = initializer.ExposedCreateOpenIdConnectEvents();

            // Assert
            events.OnRemoteFailure.Invoke(mockRemoteFailureContext.Object);

            mockHttpContext
                .Verify(
                    x => x.Response.Redirect(expectedFailurePath),
                    Times.Once
                );
        }

        [Fact]
        public void CreateOpenIdConnectEvents_Success()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(x => x.Response.Redirect("/Errors/Fault"));

            var authenticationScheme = new AuthenticationScheme(
                OpenIdConnectDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme,
                typeof(AuthenticationHandler<AuthenticationSchemeOptions>)
            );

            // OnRemoteFailure
            var mockRemoteFailureContext = new Mock<RemoteFailureContext>(
                MockBehavior.Strict,
                mockHttpContext.Object,
                authenticationScheme,
                new RemoteAuthenticationOptions(),
                new InvalidOperationException()
            );

            // OnRedirectToIdentityProvider
            var authenticationProperties = new AuthenticationProperties();

            var mockRedirectContext = new Mock<RedirectContext>(
                MockBehavior.Strict,
                mockHttpContext.Object,
                authenticationScheme,
                new OpenIdConnectOptions(),
                authenticationProperties
            );
            mockRedirectContext
                .Protected()
                .SetupSet<AuthenticationProperties>("Properties", authenticationProperties);

            var initializer = new FakeUiSecurityInitializer(
                options =>
                {
                    options.PublicAuthorityUri = new Uri("https://test.dev");
                },
                null
            );

            // Act
            var events = initializer.ExposedCreateOpenIdConnectEvents();

            // Assert
            events.OnRemoteFailure.Invoke(mockRemoteFailureContext.Object);

            mockHttpContext
                .Verify(
                    x => x.Response.Redirect("/Errors/Fault"),
                    Times.Once
                );

            mockRedirectContext.Object.ProtocolMessage = new OpenIdConnectMessage
            {
                IssuerAddress = "https://localhost:80/Home/Index?a=b"
            };
            events.OnRedirectToIdentityProvider.Invoke(mockRedirectContext.Object);
            Assert.Equal(
                "https://test.dev/Home/Index?a=b",
                mockRedirectContext.Object.ProtocolMessage.IssuerAddress
            );

            mockRedirectContext.Object.ProtocolMessage = new OpenIdConnectMessage
            {
                IssuerAddress = "https://localhost:80/Home/Index?a=b"
            };
            events.OnRedirectToIdentityProviderForSignOut.Invoke(mockRedirectContext.Object);
            Assert.Equal(
                "https://test.dev/Home/Index?a=b",
                mockRedirectContext.Object.ProtocolMessage.IssuerAddress
            );
        }
    }
}