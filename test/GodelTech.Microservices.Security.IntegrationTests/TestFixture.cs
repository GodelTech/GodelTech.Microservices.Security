using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GodelTech.Microservices.Security.SeleniumTests.Applications;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            IdentityServerApplication = new IdentityServerApplication();

            ApiApplication = new ApiApplication();
            MvcApplication = new MvcApplication();
            RazorPagesApplication = new RazorPagesApplication();

            Start();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public IdentityServerApplication IdentityServerApplication { get; }

        public ApiApplication ApiApplication { get; }

        public MvcApplication MvcApplication { get; }

        public RazorPagesApplication RazorPagesApplication { get; }

        public ServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            Stop();
        }

        public async Task AuthorizeClientAsync(HttpClient httpClient, string scope)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(IdentityServerApplication.Url.AbsoluteUri);
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

        public async Task LoginUiClientAsync(HttpClient httpClient, string username, string password)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var loginUrl = new Uri(IdentityServerApplication.Url, "Account/Login");

            var response = await httpClient.GetAsync(loginUrl);
            var responseValue = await response.Content.ReadAsStringAsync();

            var returnUrl = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(responseValue))
            {
                returnUrl = responseValue.Substring(responseValue.IndexOf("ReturnUrl", StringComparison.InvariantCulture));
                returnUrl = returnUrl.Substring(returnUrl.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("\"", StringComparison.InvariantCulture));

                verificationToken = responseValue.Substring(responseValue.IndexOf("__RequestVerificationToken", StringComparison.InvariantCulture));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\"", StringComparison.InvariantCulture));
            }

            using var contentToSend = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("ReturnUrl", returnUrl),
                    new KeyValuePair<string, string>("Username", username),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("button", "login"),
                    new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
                }
            );

            await httpClient.PostAsync(loginUrl, contentToSend);
        }

        public async Task LogoutUiClientAsync(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var logoutUrl = new Uri(IdentityServerApplication.Url, "Account/Logout");

            var response = await httpClient.GetAsync(logoutUrl);
            var responseValue = await response.Content.ReadAsStringAsync();

            var logoutId = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(responseValue))
            {
                logoutId = responseValue.Substring(responseValue.IndexOf("logoutId", StringComparison.InvariantCulture));
                logoutId = logoutId.Substring(logoutId.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                logoutId = logoutId.Substring(0, logoutId.IndexOf("\"", StringComparison.InvariantCulture));

                verificationToken = responseValue.Substring(responseValue.IndexOf("__RequestVerificationToken", StringComparison.InvariantCulture));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\"", StringComparison.InvariantCulture));
            }

            using var contentToSend = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("LogoutId", logoutId),
                    new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
                }
            );

            await httpClient.PostAsync(logoutUrl, contentToSend);
        }

        private void Start()
        {
            IdentityServerApplication.Start();

            ApiApplication.Start();
            MvcApplication.Start();
            RazorPagesApplication.Start();
        }

        private void Stop()
        {
            IdentityServerApplication.Stop();

            ApiApplication.Stop();
            MvcApplication.Stop();
            RazorPagesApplication.Stop();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            AddHttpClient(services, "ApiClient", ApiApplication.Url);
            AddHttpClient(services, "MvcClient", MvcApplication.Url);
            AddHttpClient(services, "RazorPagesClient", RazorPagesApplication.Url);
            AddHttpClient(services, "RazorPagesSecondClient", RazorPagesApplication.Url);
        }

        private static void AddHttpClient(IServiceCollection services, string name, Uri baseAddress)
        {
            services
                .AddHttpClient(
                    name,
                    client =>
                    {
                        client.BaseAddress = baseAddress;
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                );
        }
    }
}