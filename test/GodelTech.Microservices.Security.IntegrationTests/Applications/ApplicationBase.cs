using System;
using System.IO;
using System.Reflection;
using GodelTech.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public abstract class ApplicationBase<TStartup>
        where TStartup : class
    {
        private readonly IHost _host;

        protected ApplicationBase(string projectRelativePath, Uri url)
        {
            Url = url;

            var projectPath = ProjectHelpers.GetProjectPath(projectRelativePath, typeof(TStartup).GetTypeInfo().Assembly);

            _host = CreateHostBuilder(projectPath, Url).Build();
        }

        public Uri Url { get; }

        public void Start()
        {
            _host.StartAsync().GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        private static IHostBuilder CreateHostBuilder(string projectPath, Uri url)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(
                    hostConfig =>
                    {
                        hostConfig.SetBasePath(projectPath);
                        hostConfig.AddJsonFile(Path.Combine(projectPath, "appsettings.json"), false);
                    }
                )
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseUrls(url.AbsoluteUri);
                        webBuilder.UseStartup<TStartup>();

                        // todo: remove
                        webBuilder.UseEnvironment("Development");
                    }
                );
        }
    }
}