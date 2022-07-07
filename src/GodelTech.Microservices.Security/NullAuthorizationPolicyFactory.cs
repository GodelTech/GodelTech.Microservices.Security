using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security
{
    /// <summary>
    /// Empty authorization policy factory.
    /// </summary>
    public class NullAuthorizationPolicyFactory : IAuthorizationPolicyFactory
    {
        /// <inheritdoc />
        public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
        {
            return new Dictionary<string, AuthorizationPolicy>();
        }
    }
}
