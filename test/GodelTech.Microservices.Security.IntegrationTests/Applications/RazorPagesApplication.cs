﻿using System;
using GodelTech.Microservices.Security.Demo.RazorPages;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public class RazorPagesApplication : ApplicationBase<Startup>
    {
        public RazorPagesApplication()
            : base("demo", new Uri("https://localhost:44303"))
        {

        }
    }
}