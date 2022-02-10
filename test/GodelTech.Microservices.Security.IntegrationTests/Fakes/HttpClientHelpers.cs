using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Security.IntegrationTests.Fakes
{
    public static class HttpClientHelpers
    {
        public static async Task AuthorizeClientAsync(HttpClient httpClient, Uri identityServerUrl)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var loginUrl = new Uri(identityServerUrl, "Account/Login");
            
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
                    new KeyValuePair<string, string>("Username", "alice"),
                    new KeyValuePair<string, string>("Password", "alice"),
                    new KeyValuePair<string, string>("button", "login"),
                    new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
                }
            );

            await httpClient.PostAsync(loginUrl, contentToSend);
        }
    }
}