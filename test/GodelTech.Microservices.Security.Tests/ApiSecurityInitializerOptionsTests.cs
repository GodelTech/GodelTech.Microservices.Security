using System.Net;
using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class ApiSecurityInitializerOptionsTests
    {
        private readonly ApiSecurityInitializerOptions _options;

        public ApiSecurityInitializerOptionsTests()
        {
            _options = new ApiSecurityInitializerOptions();
        }

        [Fact]
        public void ClearDefaultInboundClaimTypeMap_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ClearDefaultInboundClaimTypeMap);
        }

        [Fact]
        public void ClearDefaultInboundClaimTypeMap_Set_Success()
        {
            // Arrange & Act
            _options.ClearDefaultInboundClaimTypeMap = false;

            // Assert
            Assert.False(_options.ClearDefaultInboundClaimTypeMap);
        }

        [Fact]
        public void ClearDefaultOutboundClaimTypeMap_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ClearDefaultOutboundClaimTypeMap);
        }

        [Fact]
        public void ClearDefaultOutboundClaimTypeMap_Set_Success()
        {
            // Arrange & Act
            _options.ClearDefaultOutboundClaimTypeMap = false;

            // Assert
            Assert.False(_options.ClearDefaultOutboundClaimTypeMap);
        }

        [Fact]
        public void SecurityProtocol_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Equal(SecurityProtocolType.SystemDefault, _options.SecurityProtocol);
        }

        [Fact]
#pragma warning disable CA5386 // Avoid hardcoding SecurityProtocolType value
        public void SecurityProtocol_Set_Success()
        {
            // Arrange & Act
            _options.SecurityProtocol = SecurityProtocolType.SystemDefault;

            // Assert
            Assert.Equal(SecurityProtocolType.SystemDefault, _options.SecurityProtocol);
        }
#pragma warning restore CA5386 // Avoid hardcoding SecurityProtocolType value
    }
}
