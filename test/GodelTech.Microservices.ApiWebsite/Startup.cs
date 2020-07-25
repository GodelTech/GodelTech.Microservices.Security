using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Security.Services;
using Microsoft.AspNetCore.Authorization;
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


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);

            yield return new GenericInitializer((app, env) => app.UseRouting());

           // yield return new ApiSecurityInitializer(Configuration, new PolicyFactory());

            yield return new ApiInitializer(Configuration);
        }


        private class PolicyFactory : IAuthorizationPolicyFactory
        {
            public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
            {
                return new Dictionary<string, AuthorizationPolicy>();
            }
        }
    }
}
