using System;
using System.Collections.Generic;
using System.Net.Http;
using GodelTech.Microservices.ApiWebsite;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Security.IntegrationTests.Utils;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public abstract class ApiIntegrationTestBase
    {
        protected HttpClient CreateClient(Func<IConfiguration, IEnumerable<IMicroserviceInitializer>> intializerFactory)
        {
            ApiIntegrationTestsStartup.InitializerFactory = intializerFactory;

            var factory = new IntegrationTestWebApplicationFactory<ApiIntegrationTestsStartup>();

            return factory.CreateClient();
        }
    }
}