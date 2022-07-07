using System;

namespace GodelTech.Microservices.Security.Helpers
{
    /// <summary>
    /// Url helpers.
    /// </summary>
    public static class UrlHelpers
    {
        /// <summary>
        /// Change authority to public authority keeping path an query.
        /// </summary>
        /// <param name="issuerAddress">Issuer address.</param>
        /// <param name="publicAuthorityUri">Public authority url.</param>
        /// <returns>Url.</returns>
        public static string ChangeAuthority(string issuerAddress, Uri publicAuthorityUri)
        {
            if (publicAuthorityUri == null) return issuerAddress;

            return new Uri(publicAuthorityUri, new Uri(issuerAddress).PathAndQuery).AbsoluteUri;
        }
    }
}
