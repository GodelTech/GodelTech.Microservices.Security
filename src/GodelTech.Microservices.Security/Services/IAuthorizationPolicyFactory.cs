using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Services
{
    public interface IAuthorizationPolicyFactory
    {
        IReadOnlyDictionary<string, AuthorizationPolicy> Create();
    }
}