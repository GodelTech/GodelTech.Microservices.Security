using System;
using GodelTech.Microservices.Security.Demo.Api;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public class ApiApplication : ApplicationBase<Startup>
    {
        public ApiApplication()
            : base("demo", new Uri("http://localhost:55401"))
        {

        }
    }
}