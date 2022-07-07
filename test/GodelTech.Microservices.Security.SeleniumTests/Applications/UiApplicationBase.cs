using System;
using System.Net.Http;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public abstract class UiApplicationBase<TStartup> : ApplicationBase<TStartup>
        where TStartup : class
    {
        protected UiApplicationBase(string projectRelativePath, Uri url)
            : base(projectRelativePath, url)
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

            services.Configure<HttpClientFactoryOptions>(
                AccessTokenManagementDefaults.BackChannelHttpClientName,
                options =>
                {
                    options.HttpMessageHandlerBuilderActions
                        .Add(
                            x =>
                            {
                                x.PrimaryHandler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                };
                            }
                        );
                }
            );

            services.Configure<HttpClientFactoryOptions>(
                "UserClient",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions
                        .Add(
                            x =>
                            {
                                x.PrimaryHandler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                };
                            }
                        );
                }
            );

            services.Configure<HttpClientFactoryOptions>(
                "ApiClient",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions
                        .Add(
                            x =>
                            {
                                x.PrimaryHandler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                };
                            }
                        );
                }
            );
        }
    }
}
