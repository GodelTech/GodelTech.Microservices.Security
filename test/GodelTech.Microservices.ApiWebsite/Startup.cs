using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Security;
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

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);
            yield return new GenericInitializer((app, env) => app.UseRouting());

            yield return new ApiSecurityInitializer(Configuration, new PolicyFactory());
            yield return new ApiInitializer(Configuration);
        }

        private class PolicyFactory : IAuthorizationPolicyFactory
        {
            public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
            {
                var policyBuilder = new AuthorizationPolicyBuilder();

                //policyBuilder.RequireAuthenticatedUser();
                // Check identity provider project (e.g. Config.cs) to get full list of registered scopes
                policyBuilder.RequireClaim("scope", "api1");

                return new Dictionary<string, AuthorizationPolicy>
                {
                    ["Weather API Policy"] =  policyBuilder.Build()
                };
            }
        }
    }
}
