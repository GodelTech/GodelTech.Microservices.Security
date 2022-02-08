using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace GodelTech.Microservices.Security.IntegrationTests.Fakes
{
    public static class HttpClientHelpers
    {
        public static HttpClientHandler CreateHttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }

        public static HttpClient CreateHttpClient(HttpClientHandler httpClientHandler, Uri baseAddress)
        {
            return new HttpClient(httpClientHandler, false)
            {
                BaseAddress = baseAddress
            };
        }

        public static HttpClient CreateClient(HttpClientHandler httpClientHandler, CookieContainer cookieContainer)
        {
            if (cookieContainer == null) throw new ArgumentNullException(nameof(cookieContainer));

            var uri = new Uri("https://localhost:44300/Account/Login");
            using var httpClientHandler2 = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                CookieContainer = cookieContainer
            };
            using var httpClient = new HttpClient(httpClientHandler2);

            var response = httpClient.GetAsync(uri).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var returnUrl = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                returnUrl = content.Substring(content.IndexOf("ReturnUrl", StringComparison.InvariantCulture));
                returnUrl = returnUrl.Substring(returnUrl.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("\"", StringComparison.InvariantCulture));

                verificationToken = content.Substring(content.IndexOf("__RequestVerificationToken", StringComparison.InvariantCulture));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"", StringComparison.InvariantCulture) + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\"", StringComparison.InvariantCulture));
            }
            
            using var contentToSend = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ReturnUrl", returnUrl),
                new KeyValuePair<string, string>("Username", "alice"),
                new KeyValuePair<string, string>("Password", "alice"),
                new KeyValuePair<string, string>("button", "login"),
                new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
            });
            response = httpClient.PostAsync(uri, contentToSend).Result;
            var cookies = cookieContainer.GetCookies(uri);
            cookieContainer.Add(cookies);

            var client = new HttpClient(httpClientHandler);

            return client;
        }
    }
}