using System;
using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.Demo.RazorPages
{
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {

        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer();
            yield return new ExceptionHandlerInitializer();
            yield return new HstsInitializer();

            yield return new GenericInitializer(null, (app, _) => app.UseStaticFiles());

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new UiSecurityInitializer(
                options => Configuration.Bind("UiSecurityOptions", options),
                options =>
                {
                    options.Client.DefaultClient.Scope = "api";
                }
            );

            yield return new GenericInitializer(
                services =>
                {
                    services.AddUserAccessTokenHttpClient(
                        "UserClient",
                        configureClient: client =>
                        {
                            client.BaseAddress = new Uri(Configuration["ApiUrl"]);
                        }
                    );

                    services.AddClientAccessTokenHttpClient(
                        "ApiClient",
                        configureClient: client =>
                        {
                            client.BaseAddress = new Uri(Configuration["ApiUrl"]);
                        }
                    );
                }
            );

            yield return new RazorPagesInitializer();
        }
    }
}
