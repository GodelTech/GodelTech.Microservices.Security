using System;
using System.Reflection;
using System.Threading.Tasks;
using GodelTech.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace GodelTech.Microservices.Security.SeleniumTests.Applications
{
    public abstract class ApplicationBase<TStartup> : IDisposable
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

        protected bool IsDisposed { get; private set; }

        public Uri Url { get; }

        public async Task StartAsync()
        {
            await _host.StartAsync();
        }

        public async Task StopAsync()
        {
            await _host.StopAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                // unmanaged resources would be cleaned up here.
                return;
            }

            if (IsDisposed)
            {
                // no need to dispose twice.
                return;
            }

            // free managed resources
            _host?.Dispose();

            IsDisposed = true;
        }

        #endregion
    }
}
