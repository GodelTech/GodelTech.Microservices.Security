using GodelTech.Microservices.Security.Services.Configuration;

namespace GodelTech.Microservices.Security
{
    public class ApiSecurityConfig 
    {
        public string AuthorityUri { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool RequireHttpsMetadata { get; set; } = true;

        public TokenValidationConfig TokenValidation { get; set; } = new TokenValidationConfig();
    }
}
