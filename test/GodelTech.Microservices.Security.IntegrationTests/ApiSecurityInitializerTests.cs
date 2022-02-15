using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.Demo.Api.Models.Fake;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    [Collection("TestCollection")]
    public sealed partial class ApiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClient _httpClient;

        public ApiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            var httpClientFactory = _fixture.ServiceProvider.GetService<IHttpClientFactory>();

            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public static IList<HttpRequestMessage> FakeHttpRequestMessages =>
            new Collection<HttpRequestMessage>
            {
                // get list
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("fakes", UriKind.Relative)
                },
                // get
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("fakes/1", UriKind.Relative)
                },
                // post
                new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("fakes", UriKind.Relative),
                    Content = new StringContent(
                        JsonSerializer.Serialize(
                            new FakePostModel
                            {
                                Title = "Test Title"
                            }
                        ),
                        Encoding.UTF8,
                        "application/json"
                    )
                },
                // put
                new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("fakes/1", UriKind.Relative),
                    Content = new StringContent(
                        JsonSerializer.Serialize(
                            new FakePutModel
                            {
                                Id = 1,
                                Title = "New Title"
                            }
                        ),
                        Encoding.UTF8,
                        "application/json"
                    )
                },
                // delete
                new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("fakes/1", UriKind.Relative)
                }
            };

        public static IList<HttpRequestMessage> OpenFakeHttpRequestMessages =>
            new Collection<HttpRequestMessage>
            {
                // get list
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("openFakes", UriKind.Relative)
                }
            };

        private async Task AuthorizeClientAsync(HttpClient httpClient, string scope)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(_fixture.IdentityServerApplication.Url.AbsoluteUri);
            if (discoveryDocument.IsError) throw new InvalidOperationException(discoveryDocument.Error);

            using var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "ClientForApi",
                ClientSecret = "secret",
                Scope = scope
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
            if (tokenResponse.IsError) throw new InvalidOperationException(tokenResponse.Error);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResponse.AccessToken);
        }
    }
}
