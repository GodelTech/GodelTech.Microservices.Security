using System;
using GodelTech.Microservices.Security.Demo.Api;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class ApiApplication : ApplicationBase<Startup>
    {
        public ApiApplication()
            : base("demo", new Uri("https://localhost:44301"))
        {

        }
    }
}