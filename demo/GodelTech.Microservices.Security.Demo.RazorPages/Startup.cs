using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;

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

            // todo: remove
            yield return new GenericInitializer(collection => { IdentityModelEventSource.ShowPII = true; });

            yield return new GenericInitializer(null, (app, _) => app.UseStaticFiles());

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new UiSecurityInitializer(Configuration);

            yield return new RazorPagesInitializer();
        }
    }
}
