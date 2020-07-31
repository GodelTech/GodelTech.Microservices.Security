using GodelTech.Microservices.UiWebsite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public class UiWebApplication
    {
        private readonly IHost _host;

        public UiWebApplication()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(Config.UiWebsiteUrl);
                    webBuilder.UseStartup<Startup>();
                })
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