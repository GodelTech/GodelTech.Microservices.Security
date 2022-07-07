using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Security.Demo.Api
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
            yield return new HstsInitializer();

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new ApiSecurityInitializer(
                options => Configuration.Bind("ApiSecurityOptions", options),
                new PolicyFactory()
            );

            yield return new ApiInitializer();
        }
    }
}
