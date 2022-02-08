using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using GodelTech.Microservices.Security.Demo.Api.Models.Fake;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using Xunit;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    // todo: add UseHsts for demo projects???
    [Collection("TestCollection")]
    public sealed partial class ApiSecurityInitializerTests : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;

        public ApiSecurityInitializerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _httpClientHandler = HttpClientHelpers.CreateHttpClientHandler();

            _httpClient = HttpClientHelpers.CreateHttpClient(
                _httpClientHandler,
                _fixture.ApiApplication.Url
            );
        }

        public void Dispose()
        {
            _httpClientHandler.Dispose();
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
    }
}
