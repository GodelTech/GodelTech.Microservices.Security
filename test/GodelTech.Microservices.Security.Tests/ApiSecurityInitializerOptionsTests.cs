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
        public void ClearDefaultInboundClaimTypeMap_Get()
        {
            // Arrange & Act & Assert
            Assert.True(_options.ClearDefaultInboundClaimTypeMap);
        }
    }
}