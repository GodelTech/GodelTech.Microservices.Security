using System;
using IdentityServer;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class IdentityServerApplication : ApplicationBase<Startup>
    {
        public IdentityServerApplication()
            : base("demo", new Uri("https://localhost:44300"))
        {

        }
    }
}