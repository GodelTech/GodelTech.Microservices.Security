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
            Action<AccessTokenManagementOptions> configureAccessTokenManagement,
            string failurePath)
            : base(configureUiSecurity, configureAccessTokenManagement, failurePath)
        {

        }

        public FakeUiSecurityInitializer(
            Action<UiSecurityOptions> configureUiSecurity,
            Action<AccessTokenManagementOptions> configureAccessTokenManagement)
            : base(configureUiSecurity, configureAccessTokenManagement)
        {

        }

        public void ExposedConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            base.ConfigureOpenIdConnectOptions(options);
        }

        public OpenIdConnectEvents ExposedCreateOpenIdConnectEvents()
        {
            return base.CreateOpenIdConnectEvents();
        }
    }
}
