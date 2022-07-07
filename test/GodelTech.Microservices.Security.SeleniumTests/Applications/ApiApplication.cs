using System;
using System.Net.Http;
using GodelTech.Microservices.Security.Demo.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class ApiApplication : ApplicationBase<Startup>
    {
        public ApiApplication()
            : base("demo", new Uri("https://localhost:44301"))
        {

        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.Configure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
            );
        }
    }
}
