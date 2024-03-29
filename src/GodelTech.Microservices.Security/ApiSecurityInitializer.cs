﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.ApiSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

[assembly: CLSCompliant(false)]
namespace GodelTech.Microservices.Security
{
    /// <summary>
    /// ApiSecurity initializer.
    /// </summary>
    public class ApiSecurityInitializer : IMicroserviceInitializer
    {
        private readonly ApiSecurityOptions _apiSecurityOptions = new ApiSecurityOptions();
        private readonly IAuthorizationPolicyFactory _policyFactory;
        private readonly ApiSecurityInitializerOptions _options = new ApiSecurityInitializerOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSecurityInitializer"/> class.
        /// </summary>
        /// <param name="configureApiSecurity">An <see cref="Action{ApiSecurityOptions}"/> to configure the provided <see cref="ApiSecurityOptions"/>.</param>
        /// <param name="configure">An <see cref="Action{ApiSecurityInitializerOptions}"/> to configure the provided <see cref="ApiSecurityInitializerOptions"/>.</param>
        public ApiSecurityInitializer(
            Action<ApiSecurityOptions> configureApiSecurity,
            Action<ApiSecurityInitializerOptions> configure = null)
            : this(configureApiSecurity, new NullAuthorizationPolicyFactory(), configure)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSecurityInitializer"/> class.
        /// </summary>
        /// <param name="configureApiSecurity">An <see cref="Action{ApiSecurityOptions}"/> to configure the provided <see cref="ApiSecurityOptions"/>.</param>
        /// <param name="policyFactory">The authorization policy factory.</param>
        /// <param name="configure">An <see cref="Action{ApiSecurityInitializerOptions}"/> to configure the provided <see cref="ApiSecurityInitializerOptions"/>.</param>
        public ApiSecurityInitializer(
            Action<ApiSecurityOptions> configureApiSecurity,
            IAuthorizationPolicyFactory policyFactory,
            Action<ApiSecurityInitializerOptions> configure = null)
        {
            ArgumentNullException.ThrowIfNull(configureApiSecurity);
            ArgumentNullException.ThrowIfNull(policyFactory);

            configureApiSecurity.Invoke(_apiSecurityOptions);

            _policyFactory = policyFactory;

            configure?.Invoke(_options);
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(ConfigureJwtBearerOptions);

            services.AddAuthorization(ConfigureAuthorizationOptions);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (_options.ClearDefaultInboundClaimTypeMap)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            }

            if (_options.ClearDefaultOutboundClaimTypeMap)
            {
                JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            }

            ServicePointManager.SecurityProtocol = _options.SecurityProtocol;

            app.UseAuthentication();
            app.UseAuthorization();
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        /// <summary>
        /// Configure JwtBearerOptions.
        /// </summary>
        /// <param name="options">JwtBearerOptions.</param>
        protected virtual void ConfigureJwtBearerOptions(JwtBearerOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.RequireHttpsMetadata = _apiSecurityOptions.RequireHttpsMetadata;
            options.Authority = _apiSecurityOptions.Authority;
            options.Audience = _apiSecurityOptions.Audience;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = _apiSecurityOptions.TokenValidation.ValidateAudience,
                ValidateIssuer = _apiSecurityOptions.TokenValidation.ValidateIssuer,
                ValidateIssuerSigningKey = _apiSecurityOptions.TokenValidation.ValidateIssuerSigningKey,
                ValidateLifetime = _apiSecurityOptions.TokenValidation.ValidateLifetime,
                ValidAudience = _apiSecurityOptions.Audience,
                ValidIssuer = _apiSecurityOptions.Issuer
            };

            options.SaveToken = _apiSecurityOptions.SaveToken;
            options.IncludeErrorDetails = _apiSecurityOptions.IncludeErrorDetails;
        }

        /// <summary>
        /// Configure AuthorizationOptions.
        /// </summary>
        /// <param name="options">AuthorizationOptions.</param>
        protected virtual void ConfigureAuthorizationOptions(AuthorizationOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            foreach (var policy in _policyFactory.Create())
            {
                options.AddPolicy(policy.Key, policy.Value);
            }
        }
    }
}
