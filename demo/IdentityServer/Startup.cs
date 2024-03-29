﻿using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
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

            yield return new IdentityServerInitializer();

            yield return new GenericInitializer(null, (app, _) => app.UseAuthorization());

            yield return new MvcInitializer();
        }
    }
}
