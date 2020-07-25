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
        private readonly IAuthorizationPolicyFactory _policyFactory;

        public SecurityProtocolType SecurityProtocol { get; set; } = SecurityProtocolType.Tls12;
        public bool ClearDefaultInboundClaimTypeMap { get; set; } = true;
        public bool ClearDefaultOutboundClaimTypeMap { get; set; } = true;

        public ApiSecurityInitializer(IConfiguration configuration)
            : this(configuration, new NullAuthorizationPolicyFactory())
        {
        }

        public ApiSecurityInitializer(IConfiguration configuration, IAuthorizationPolicyFactory policyFactory)
            : base(configuration)
        {
            _policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

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
            if (services == null) 
                throw new ArgumentNullException(nameof(services));

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
            var config = new ApiSecurityConfig();

            Configuration.Bind("ApiSecurityConfig", config);

            options.Authority = config.AuthorityUri;
            options.Audience = config.Audience;
            options.IncludeErrorDetails = true;
            options.RequireHttpsMetadata = config.RequireHttpsMetadata;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = config.TokenValidation.ValidateIssuer,
                    ValidateAudience = config.TokenValidation.ValidateAudience,
                    ValidateLifetime = config.TokenValidation.ValidateLifetime,
                    ValidateIssuerSigningKey = config.TokenValidation.ValidateIssuerSigningKey,
                    ValidIssuer = config.Issuer,
                    ValidAudience = options.Audience,
                };

            options.SaveToken = true;
        }

        protected virtual void ConfigureAuthorization(AuthorizationOptions options)
        {
            foreach (var (policyName, policy) in _policyFactory.Create())
            {
                options.AddPolicy(policyName, policy);
            }
        }
    }
}
