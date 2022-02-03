using System;
using System.Reflection;
using GodelTech.Microservices.Security.IntegrationTests.Fakes;
using IdentityServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public class IdentityServerApplication
    {
        public static readonly Uri Url = new Uri("http://localhost:55400");

        private readonly IHost _host;

        // todo: is it ok in constructor?
        public IdentityServerApplication()
        {
            var projectPath = ProjectHelpers.GetProjectPath("demo", typeof(Demo.Api.Startup).GetTypeInfo().Assembly);

            _host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(
                    hostConfig =>
                    {
                        hostConfig.SetBasePath(projectPath);
                        // todo: appsettings.json not working
                        hostConfig.AddJsonFile("appsettings.json");
                    }
                )
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseUrls(Url.AbsoluteUri);
                        webBuilder.UseStartup<Startup>();
                    }
                )
                .Build();
        }

        public void Start()
        {
            _host.StartAsync().GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }
    }
}