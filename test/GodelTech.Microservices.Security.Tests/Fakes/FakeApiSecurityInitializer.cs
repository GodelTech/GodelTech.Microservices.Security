using System;
using GodelTech.Microservices.Security.ApiSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Security.Tests.Fakes
{
    public class FakeApiSecurityInitializer : ApiSecurityInitializer
    {
        public FakeApiSecurityInitializer(
            Action<ApiSecurityOptions> configureApiSecurity,
            Action<ApiSecurityInitializerOptions> configure = null) 
            : base(configureApiSecurity, configure)
        {

        }

        public FakeApiSecurityInitializer(
            Action<ApiSecurityOptions> configureApiSecurity,
            IAuthorizationPolicyFactory policyFactory,
            Action<ApiSecurityInitializerOptions> configure = null) 
            : base(configureApiSecurity, policyFactory, configure)
        {

        }

        public void ExposedConfigureJwtBearerOptions(JwtBearerOptions options)
        {
            base.ConfigureJwtBearerOptions(options);
        }

        public void ExposedConfigureAuthorizationOptions(AuthorizationOptions options)
        {
            base.ConfigureAuthorizationOptions(options);
        }
    }
}