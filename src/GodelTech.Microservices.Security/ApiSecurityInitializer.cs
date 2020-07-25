using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GodelTech.Microservices.Security
{
    public class ApiSecurityInitializer : MicroserviceInitializerBase
    {
        private readonly ISecurityInfoProvider _securityInfoProvider;

        public SecurityProtocolType SecurityProtocol { get; set; } = SecurityProtocolType.Tls12;
        public bool ClearDefaultInboundClaimTypeMap { get; set; } = true;
        public bool ClearDefaultOutboundClaimTypeMap { get; set; } = true;

        public ApiSecurityInitializer(IConfiguration configuration)
            : this(configuration, new NullSecurityInfoProvider())
        {
        }

        public ApiSecurityInitializer(IConfiguration configuration, ISecurityInfoProvider securityInfoProvider)
            : base(configuration)
        {
            _securityInfoProvider = securityInfoProvider ?? throw new ArgumentNullException(nameof(securityInfoProvider));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (ClearDefaultInboundClaimTypeMap)
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (ClearDefaultOutboundClaimTypeMap)
                JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            ServicePointManager.SecurityProtocol = SecurityProtocol;

            app.UseAuthentication();
            app.UseAuthorization();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Default scheme name is JwtBearerDefaults.AuthenticationScheme
                // You can check overloads of AddJwtBearer() for find this information
                .AddJwtBearer(ConfigureJwtBearerOptions);

            services.AddAuthorization(ConfigureAuthorization);
        }

        protected virtual void ConfigureJwtBearerOptions(JwtBearerOptions options)
        {
            var config = Configuration.GetIdentityConfiguration();

            options.Authority = config.AuthorityUri;
            options.Audience = config.Audience;
            options.IncludeErrorDetails = true;
            options.RequireHttpsMetadata = config.RequireHttpsMetadata;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.Issuer,
                    ValidAudience = options.Audience,
                };

            options.SaveToken = true;
        }

        protected virtual void ConfigureAuthorization(AuthorizationOptions options)
        {
            foreach (var (policyName, policy) in _securityInfoProvider.CreatePolicies())
            {
                options.AddPolicy(policyName, policy);
            }
        }
    }
}
