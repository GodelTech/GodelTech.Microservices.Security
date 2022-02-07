using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class NullAuthorizationPolicyFactoryTests
    {
        [Fact]
        public void Create_Success()
        {
            // Arrange
            var policyFactory = new NullAuthorizationPolicyFactory();

            // Act
            var result = policyFactory.Create();

            // Assert
            result.Should().BeEquivalentTo(new Dictionary<string, AuthorizationPolicy>());
        }
    }
}