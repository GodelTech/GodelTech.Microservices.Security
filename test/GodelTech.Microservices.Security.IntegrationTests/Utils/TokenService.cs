using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace GodelTech.Microservices.Security.IntegrationTests.Utils
{
    public class TokenService
    {
        private readonly Uri _authorityUrl;

        public TokenService(Uri authorityUrl)
        {
            _authorityUrl = authorityUrl;
        }

        public Task<string> GetClientCredentialsTokenAsync(string clientId, string clientSecret, params string[] scopes)
        {
            return GetTokenAsync(
                (client, disco) =>
                {
                    var tokenRequest = new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,

                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = string.Join(' ', scopes)
                    };

                    return client.RequestClientCredentialsTokenAsync(tokenRequest);
                }
            );
        }

        private async Task<string> GetTokenAsync(Func<HttpClient, DiscoveryDocumentResponse, Task<TokenResponse>> tokenResponseProvider)
        {
            using (var client = new HttpClient())
            {
                var discoveryDocumentRequest = new DiscoveryDocumentRequest
                {
                    Address = _authorityUrl.AbsoluteUri,
                    Policy =
                    {
                        ValidateIssuerName = false,
                        RequireHttps = false
                    }
                };

                var disco = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest);
                if (disco.IsError)
                    throw new Exception(disco.Error);

                var response = await tokenResponseProvider(client, disco);

                if (response.IsError)
                    throw new Exception(response.Error);

                return response.AccessToken;
            }
        }
    }
}