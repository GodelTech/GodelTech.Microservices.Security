using System;
using System.Net.Http;
using GodelTech.Microservices.Security.Demo.RazorPages;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class RazorPagesApplication : ApplicationBase<Startup>
    {
        public RazorPagesApplication()
            : base("demo", new Uri("https://localhost:44303"))
        {

        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    // CookieContainer doesn't allow cookies from other paths
                    // https://github.com/IdentityServer/IdentityServer4/issues/2925
                    // https://github.com/dotnet/aspnetcore/blob/d8381656429addead2e5eb22ba1356abfb419d86/src/Azure/AzureAD/test/FunctionalTests/WebAuthenticationTests.cs#L192-L194
                    options.CorrelationCookie.Path = "/";
                    options.NonceCookie.Path = "/";
                }
            );
        }
    }
}