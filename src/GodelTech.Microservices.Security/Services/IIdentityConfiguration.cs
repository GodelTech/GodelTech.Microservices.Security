using System.Collections.Generic;

namespace GodelTech.Microservices.Security.Services
{
    public interface IIdentityConfiguration
    {
        string AuthorityUri { get; }
        string Audience { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Scopes { get; }
        string Issuer { get; }
        string PublicAuthorityUri { get; }
        IEnumerable<string> ScopesAsList { get; }
        bool RequireHttpsMetadata { get; }
    }
}