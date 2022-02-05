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

        public static HttpClient CreateClient(CookieContainer cookieContainer)
        {
            var uri = new Uri("https://localhost:44300/Account/Login");
            var httpClientHandler = new HttpClientHandler
            {
                // todo: solve this
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                //
                CookieContainer = cookieContainer
            };
            HttpClient httpClient = new HttpClient(httpClientHandler, false);


            var response = httpClient.GetAsync("https://localhost:44300/Account/Login").Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var returnUrl = string.Empty;
            var verificationToken = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                returnUrl = content.Substring(content.IndexOf("ReturnUrl"));
                returnUrl = returnUrl.Substring(returnUrl.IndexOf("value=\"") + 7);
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("\""));

                verificationToken = content.Substring(content.IndexOf("__RequestVerificationToken"));
                verificationToken = verificationToken.Substring(verificationToken.IndexOf("value=\"") + 7);
                verificationToken = verificationToken.Substring(0, verificationToken.IndexOf("\""));
            }


            var contentToSend = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ReturnUrl", returnUrl),
                new KeyValuePair<string, string>("Username", "alice"),
                new KeyValuePair<string, string>("Password", "alice"),
                new KeyValuePair<string, string>("button", "login"),
                new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
            });
            response = httpClient.PostAsync("https://localhost:44300/Account/Login", contentToSend).Result;
            var cookies = cookieContainer.GetCookies(new Uri("https://localhost:44300/Account/Login"));
            cookieContainer.Add(cookies);

            var httpClientHandler2 = new HttpClientHandler
            {
                // todo: solve this
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                //
                //AllowAutoRedirect = false,
                CookieContainer = cookieContainer
            };
            var client = new HttpClient(httpClientHandler2, false);

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