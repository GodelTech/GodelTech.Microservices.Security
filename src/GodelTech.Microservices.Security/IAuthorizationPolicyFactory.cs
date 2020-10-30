using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security
{
    public interface IAuthorizationPolicyFactory
    {
        IReadOnlyDictionary<string, AuthorizationPolicy> Create();
    }
}
