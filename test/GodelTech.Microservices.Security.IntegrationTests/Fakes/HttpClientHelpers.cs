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

        public static HttpClient CreateClient()
        {
            var cookieContainer = new CookieContainer();
            var uri = new Uri("https://localhost:44300/Account/Login");
            var httpClientHandler = new HttpClientHandler
            {
                // todo: solve this
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                //
                CookieContainer = cookieContainer
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            var verificationToken = GetVerificationToken(httpClient, "https://localhost:44300/Account/Login");
            var contentToSend = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "alice"),
                new KeyValuePair<string, string>("Password", "alice"),
                new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
            });
            var response = httpClient.PostAsync("https://localhost:44300/Account/Login", contentToSend).Result;
            var cookies = cookieContainer.GetCookies(new Uri("https://localhost:44300/Account/Login"));
            cookieContainer.Add(cookies);
            var client = new HttpClient(httpClientHandler);

            return client;
        }

        private static string GetVerificationToken(HttpClient client, string url)
        {
            var response = client.GetAsync(url).Result;
            var verificationToken = response.Content.ReadAsStringAsync().Result;
            if (verificationToken != null && verificationToken.Length > 0)
            {
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("__RequestVerificationToken"));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"") + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\""));
            }
            return verificationToken;
        }
    }
}