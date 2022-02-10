using System;
using GodelTech.Microservices.Security.UiSecurity;
using IdentityModel.AspNetCore.AccessTokenManagement;

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

    }
}