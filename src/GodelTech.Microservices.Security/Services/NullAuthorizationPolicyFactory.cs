using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Services
{
    public class NullAuthorizationPolicyFactory : IAuthorizationPolicyFactory
    {
        public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
        {
            return new Dictionary<string, AuthorizationPolicy>();
        }
    }
}