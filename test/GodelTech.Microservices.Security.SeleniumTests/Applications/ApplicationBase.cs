using System;
using System.Reflection;
using GodelTech.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public abstract class ApplicationBase<TStartup>
        where TStartup : class
    {
        private readonly IHost _host;

        protected ApplicationBase(string projectRelativePath, Uri url)
        {
            Url = url;

            var projectPath = ProjectHelpers.GetProjectPath(projectRelativePath, typeof(TStartup).GetTypeInfo().Assembly);

            _host = CreateHostBuilder(projectPath, Url)
                .ConfigureServices(ConfigureServices)
                .Build();
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

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Flag which indicates whether or not PII is shown in logs. False by default.
            IdentityModelEventSource.ShowPII = true;
        }

        private static IHostBuilder CreateHostBuilder(string projectPath, Uri url)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(
                    builder =>
                    {
                        builder.SetBasePath(projectPath);
                        builder.AddJsonFile("appsettings.json", false);
                    }
                )
                .ConfigureAppConfiguration(
                    builder =>
                    {
                        builder.SetBasePath(projectPath);
                        builder.AddJsonFile("appsettings.json", false);
                    }
                )
                .ConfigureWebHostDefaults(
                    builder =>
                    {
                        builder.UseUrls(url.AbsoluteUri);
                        builder.UseStartup<TStartup>();
                    }
                );
        }
    }
}