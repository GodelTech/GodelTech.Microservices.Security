using System;

namespace GodelTech.Microservices.Security
{
    public class UiSecurityConfig
    {
        public string AuthorityUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Issuer { get; set; }
        public string PublicAuthorityUri { get; set; }
        public string ResponseType { get; set; } = "code";
        public bool RequireHttpsMetadata { get; set; } = true;
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }
}
