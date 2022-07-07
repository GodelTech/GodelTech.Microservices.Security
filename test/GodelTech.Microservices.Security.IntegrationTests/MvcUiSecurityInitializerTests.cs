using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class MvcUiSecurityInitializerTests : UiSecurityInitializerTests
    {
        public MvcUiSecurityInitializerTests(TestFixture fixture)
            : base(
                fixture,
                fixture?.MvcApplication.Url,
                "MvcClient",
                "MvcSecondClient")
        {

        }
    }
}
