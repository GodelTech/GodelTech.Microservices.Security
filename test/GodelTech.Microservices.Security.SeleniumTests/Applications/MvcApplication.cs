using System;
using GodelTech.Microservices.Security.Demo.Mvc;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class MvcApplication : UiApplicationBase<Startup>
    {
        public MvcApplication()
            : base("demo", new Uri("https://localhost:44302"))
        {

        }
    }
}
