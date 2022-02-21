using Microsoft.IdentityModel.Tokens;

namespace GodelTech.Microservices.Security.ApiSecurity
{
    /// <summary>
    /// TokenValidation options.
    /// </summary>
    public class TokenValidationOptions
    {
        /// <summary>
        /// Gets or sets a boolean to control if the audience will be validated during token validation.
        /// </summary>
        /// <remarks>Validation of the audience, mitigates forwarding attacks. For example, a site that receives a token, could not replay it to another side.
        /// A forwarded token would contain the audience of the original site.</remarks>
        public bool ValidateAudience { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean to control if the issuer will be validated during token validation.
        /// </summary>
        /// <remarks>
        /// Validation of the issuer mitigates forwarding attacks that can occur when an
        /// IdentityProvider represents multiple tenants and signs tokens with the same keys.
        /// It is possible that a token issued for the same audience could be from a different tenant. For example an application could accept users from
        /// contoso.onmicrosoft.com but not fabrikam.onmicrosoft.com, both valid tenants. A application that accepts tokens from fabrikam could forward them
        /// to the application that accepts tokens for contoso.
        /// </remarks>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean that controls if validation of the <see cref="SecurityKey"/> that signed the securityToken is called.
        /// </summary>
        /// <remarks>It is possible for tokens to contain the public key needed to check the signature. For example, X509Data can be hydrated into an X509Certificate,
        /// which can be used to validate the signature. In these cases it is important to validate the SigningKey that was used to validate the signature. </remarks>
        public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean to control if the lifetime will be validated during token validation.
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;
    }
}