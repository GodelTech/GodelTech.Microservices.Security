using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Services
{
    public interface ISecurityInfoProvider
    {
        IReadOnlyDictionary<string, AuthorizationPolicy> CreatePolicies();
    }
}