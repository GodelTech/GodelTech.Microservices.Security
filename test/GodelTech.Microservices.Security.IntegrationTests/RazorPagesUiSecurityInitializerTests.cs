using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed class RazorPagesUiSecurityInitializerTests : UiSecurityInitializerTests
    {
        public RazorPagesUiSecurityInitializerTests(TestFixture fixture)
            : base(
                fixture,
                fixture?.RazorPagesApplication.Url,
                "RazorPagesClient",
                "RazorPagesSecondClient")
        {

        }
    }
}