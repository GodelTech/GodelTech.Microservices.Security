using GodelTech.Microservices.ApiWebsite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Security.IntegrationTests.Applications
{
    public class ApiWebApplication
    {
        private readonly IHost _host;

        public ApiWebApplication()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(Config.ApiWebsiteUrl);
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