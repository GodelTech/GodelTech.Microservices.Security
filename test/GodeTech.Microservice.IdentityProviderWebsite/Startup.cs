using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GodeTech.Microservice.IdentityProviderWebsite
{
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);

            yield return new GenericInitializer((app, env) => app.UseStaticFiles());
            yield return new GenericInitializer((app, env) => app.UseRouting());

            yield return new IdentityServerInitializer(Configuration);

            yield return new GenericInitializer((app, env) => app.UseAuthentication());
            yield return new MvcInitializer(Configuration);
        }
    }
}
