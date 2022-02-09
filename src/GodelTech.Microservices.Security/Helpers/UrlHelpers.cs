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
        /// <param name="authority">Authority url.</param>
        /// <param name="publicAuthorityUri">Public authority url.</param>
        /// <returns>Url.</returns>
        public static string ChangeAuthority(string authority, Uri publicAuthorityUri)
        {
            if (publicAuthorityUri == null) return authority;

            return new Uri(publicAuthorityUri, new Uri(authority).PathAndQuery).AbsoluteUri;
        }
    }
}