using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Demo.Api
{
    public class PolicyFactory : IAuthorizationPolicyFactory
    {
        public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
        {
            return new Dictionary<string, AuthorizationPolicy>
            {
                ["add"] = GetAuthorizationPolicy("fake.add"),
                ["edit"] = GetAuthorizationPolicy("fake.edit"),
                ["delete"] = GetAuthorizationPolicy("fake.delete")
            };
        }

        private static AuthorizationPolicy GetAuthorizationPolicy(string requiredScope)
        {
            var policyBuilder = new AuthorizationPolicyBuilder();

            policyBuilder.RequireAuthenticatedUser();
            policyBuilder.RequireClaim("scope", requiredScope);

            return policyBuilder.Build();
        }
    }
}