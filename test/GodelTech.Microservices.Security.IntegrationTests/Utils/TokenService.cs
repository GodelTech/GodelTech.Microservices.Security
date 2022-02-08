using System;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using IdentityModel.Client;

namespace GodelTech.Microservices.Security.IntegrationTests.Utils
{
    // todo: use real TokenService from IdentityModel.AspNetCore or from GodelTech library https://github.com/GodelTech/GodelTech.Auth.IdentityModel
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
            using var httpClientHandler = HttpClientHelpers.CreateHttpClientHandler();

            using var httpClient = new HttpClient(httpClientHandler);

            var discoveryDocumentRequest = new DiscoveryDocumentRequest
            {
                Address = _authorityUrl.AbsoluteUri,
                Policy =
                {
                    ValidateIssuerName = false,
                    RequireHttps = false
                }
            };

            var disco = await httpClient.GetDiscoveryDocumentAsync(discoveryDocumentRequest);
            if (disco.IsError)
                throw new Exception(disco.Error);

            var response = await tokenResponseProvider(httpClient, disco);

            if (response.IsError)
                throw new Exception(response.Error);

            return response.AccessToken;
        }
    }
}