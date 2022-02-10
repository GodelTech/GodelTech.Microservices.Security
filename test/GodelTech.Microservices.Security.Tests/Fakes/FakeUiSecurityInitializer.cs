using System;
using GodelTech.Microservices.Security.UiSecurity;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GodelTech.Microservices.Security.Tests.Fakes
{
    public class FakeUiSecurityInitializer : UiSecurityInitializer
    {
        public FakeUiSecurityInitializer(
            Action<UiSecurityOptions> configureUiSecurity,
            Action<AccessTokenManagementOptions> configureAccessTokenManagement = null,
            string failurePath = "/Errors/Fault") 
            : base(configureUiSecurity, configureAccessTokenManagement, failurePath)
        {

        }

        public void ExposedConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            base.ConfigureOpenIdConnectOptions(options);
        }
    }
}