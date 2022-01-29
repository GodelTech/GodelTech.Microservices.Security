using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.UiWebsite
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

            yield return new GenericInitializer(null, (app, _) => app.UseStaticFiles());


            yield return new GenericInitializer(null, (app, _) => app.UseRouting());
            yield return new UiSecurityInitializer(Configuration);

            yield return new GenericInitializer(null, (app, _) => app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapRazorPages();
                })
            );

            yield return new RazorPagesInitializer();
        }
    }
}