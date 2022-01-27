using System.Collections.Generic;
using GodelTech.Microservices.Security.Services;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.ApiWebsite
{
    public class PolicyFactory : IAuthorizationPolicyFactory
    {
        public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();

            policyBuilder.RequireAuthenticatedUser();
            // Check identity provider project (e.g. Config.cs) to get full list of registered scopes
            policyBuilder.RequireClaim("scope", "api1");

            return new Dictionary<string, AuthorizationPolicy>
            {
                ["Weather API Policy"] = policyBuilder.Build()
            };
        }
    }
}