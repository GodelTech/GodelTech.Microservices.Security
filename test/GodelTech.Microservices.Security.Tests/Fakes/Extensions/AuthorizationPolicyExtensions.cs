using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Tests.Fakes.Extensions
{
    public static class AuthorizationPolicyExtensions
    {
        public static AuthorizationPolicy GetAuthorizationPolicy(string requiredScope)
        {
            var policyBuilder = new AuthorizationPolicyBuilder();

            policyBuilder.RequireAuthenticatedUser();
            policyBuilder.RequireClaim("scope", requiredScope);

            return policyBuilder.Build();
        }
    }
}