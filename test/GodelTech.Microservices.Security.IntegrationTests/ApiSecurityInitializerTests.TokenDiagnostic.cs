using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public partial class ApiSecurityInitializerTests
    {
        [Fact]
        public async Task TokenDiagnostic_DefaultInboundClaimTypeMaps()
        {
            // Arrange & Act
            var result = await _httpClient.GetAsync(
                new Uri(
                    "tokendiagnostics/DefaultInboundClaimTypeMaps",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            Assert.Equal(
                "{}",
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task TokenDiagnostic_DefaultOutboundClaimTypeMap()
        {
            // Arrange & Act
            var result = await _httpClient.GetAsync(
                new Uri(
                    "tokendiagnostics/DefaultOutboundClaimTypeMap",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            Assert.Equal(
                "{}",
                await result.Content.ReadAsStringAsync()
            );
        }
    }
}
