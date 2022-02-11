using System;
using System.Collections.Generic;
using FluentAssertions;
using GodelTech.Microservices.Security.Tests.Fakes;
using GodelTech.Microservices.Security.UiSecurity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class UiSecurityInitializerTests
    {
        private readonly FakeUiSecurityInitializer _initializer;

        public UiSecurityInitializerTests()
        {
            _initializer = new FakeUiSecurityInitializer(
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

            var initializer = new FakeUiSecurityInitializer(configureUiSecurity);

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

        [Fact]
        public void CreateOpenIdConnectEvents_WhenPublicAuthorityUriIsNull()
        {
            //// Arrange
            //var mockHttpContext = new Mock<HttpContext>(MockBehavior.Default);

            //var a = new AuthenticationScheme(
            //    OpenIdConnectDefaults.AuthenticationScheme,
            //    OpenIdConnectDefaults.AuthenticationScheme,
            //    ) new AuthenticationSchemeBuilder(OpenIdConnectDefaults.AuthenticationScheme).Build();

            //var remoteFailureContext = new RemoteFailureContext(
            //    mockHttpContext.Object,
            //    a,
            //    new RemoteAuthenticationOptions(),
            //    new InvalidOperationException()
            //);

            //var expectedResult = new OpenIdConnectEvents
            //{
            //    OnRemoteFailure = context =>
            //    {
            //        context.Response.Redirect("/Errors/Fault");
            //        context.HandleResponse();

            //        return Task.CompletedTask;
            //    }
            //};

            //// Act
            //var events = _initializer.ExposedCreateOpenIdConnectEvents();

            //// Assert
            //events.OnRemoteFailure.Invoke(remoteFailureContext);

            //// todo: solve this
            ////Assert.Equal("/Errors/Fault", mockRemoteFailureContext.Object.Response.);
        }

        [Fact]
        public void CreateOpenIdConnectEvents_Success()
        {

        }
    }
}