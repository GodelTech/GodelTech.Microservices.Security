using System;
using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Security.Demo.Mvc
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
            yield return new ExceptionHandlerInitializer("/Home/Error");
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
                    services.AddClientAccessTokenHttpClient("user_client", configureClient: client =>
                    {
                        //client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
                    });
                    //// registers HTTP client that uses the managed user access token
                    //services.AddClientAccessTokenHttpClient(
                    //    "user_client",
                    //    configureClient: client =>
                    //    {
                    //        //client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
                    //    }
                    //);
                }
            );

            yield return new MvcInitializer();
        }
    }
}
