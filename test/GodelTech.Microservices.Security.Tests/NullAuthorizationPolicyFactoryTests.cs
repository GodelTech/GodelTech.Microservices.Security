using Xunit;

namespace GodelTech.Microservices.Security.Tests
{
    public class NullAuthorizationPolicyFactoryTests
    {
        private readonly NullAuthorizationPolicyFactory _policyFactory;

        public NullAuthorizationPolicyFactoryTests()
        {
            _policyFactory = new NullAuthorizationPolicyFactory();
        }

        [Fact]
        public void Create_Success()
        {
            // Arrange & Act & Assert
            Assert.Empty(_policyFactory.Create());
        }
    }
}