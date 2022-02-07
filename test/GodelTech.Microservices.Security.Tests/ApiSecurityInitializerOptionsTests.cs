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
            Assert.Equal(SecurityProtocolType.Tls12, _options.SecurityProtocol);
        }

        [Fact]
        public void SecurityProtocol_Set_Success()
        {
            // Arrange & Act
            _options.SecurityProtocol = SecurityProtocolType.Tls13;

            // Assert
            Assert.Equal(SecurityProtocolType.Tls13, _options.SecurityProtocol);
        }
    }
}