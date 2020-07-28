namespace GodelTech.Microservices.Security
{
    public class UiSecurityConfig
    {
        public string AuthorityUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Issuer { get; set; }
        public string[] Scopes { get; set; }
        public string PublicAuthorityUri { get; set; }
    }
}