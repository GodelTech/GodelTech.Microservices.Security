using System;
using GodelTech.Microservices.Security.Demo.RazorPages;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public class MvcApplication : ApplicationBase<Startup>
    {
        public MvcApplication()
            : base("demo", new Uri("https://localhost:44302"))
        {

        }
    }
}