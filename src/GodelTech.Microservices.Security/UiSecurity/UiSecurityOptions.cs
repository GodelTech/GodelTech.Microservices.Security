﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GodelTech.Microservices.Security.UiSecurity
{
    // todo: order properties before writing tests
    /// <summary>
    /// UiSecurity options.
    /// </summary>
    public class UiSecurityOptions
    {
        /// <summary>
        /// Gets or sets the authentication scheme corresponding to the middleware
        /// responsible of persisting user's identity after a successful authentication.
        /// This value typically corresponds to a cookie middleware registered in the Startup class.
        /// When omitted, <see cref="AuthenticationOptions.DefaultSignInScheme"/> is used as a fallback value.
        /// </summary>
        public string SignInScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>
        /// Gets or sets the 'response_type'.
        /// </summary>
        public string ResponseType { get; set; } = OpenIdConnectResponseType.Code;

        /// <summary>
        /// Gets or sets if HTTPS is required for the metadata address or authority.
        /// The default is true. This should be disabled only in development environments.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Defines whether access and refresh tokens should be stored in the
        /// <see cref="AuthenticationProperties"/> after a successful authorization.
        /// This property is set to <c>false</c> by default to reduce
        /// the size of the final authentication cookie.
        /// </summary>
        public bool SaveTokens { get; set; } = true;

        /// <summary>
        /// Boolean to set whether the handler should go to user info endpoint to retrieve additional claims or not after creating an identity from id_token received from token endpoint.
        /// The default is 'false'.
        /// </summary>
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;

        /// <summary>
        /// Gets or sets the Authority to use when making OpenIdConnect calls.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Public authority url.
        /// </summary>
        public Uri PublicAuthorityUri { get; set; }

        /// <summary>
        /// Gets or sets the issuer that will be used to check against the token's issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the 'client_id'.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the 'client_secret'.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// A map between the scope name and a short description for it.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        // You can suppress the warning if the property is part of a Data Transfer Object (DTO) class.
        public ICollection<string> Scopes { get; set; } = new Collection<string>();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}