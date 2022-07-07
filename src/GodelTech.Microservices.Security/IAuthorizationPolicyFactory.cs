using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security
{
    /// <summary>
    /// Authorization policy factory.
    /// </summary>
    public interface IAuthorizationPolicyFactory
    {
        /// <summary>
        /// Create collection of <see cref="AuthorizationPolicy"/>.
        /// </summary>
        /// <returns>Collection of <see cref="AuthorizationPolicy"/>.</returns>
        IReadOnlyDictionary<string, AuthorizationPolicy> Create();
    }
}
