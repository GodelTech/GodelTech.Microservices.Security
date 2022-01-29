using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.ApiWebsite
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

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new ApiSecurityInitializer(Configuration, new PolicyFactory());
            yield return new GenericInitializer(null, (app, _) => app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                })
            );

            yield return new ApiInitializer();
        }
    }
}
