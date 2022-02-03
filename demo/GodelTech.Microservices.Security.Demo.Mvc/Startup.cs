using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

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

            yield return new GenericInitializer(null, (app, _) => app.UseStaticFiles());

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new UiSecurityInitializer(Configuration);

            yield return new MvcInitializer();
        }
    }
}
