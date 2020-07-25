using GodeTech.Microservice.IdentityProviderWebsite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Security.IntegrationTests
{
    public class IdentityProviderApplication
    {
        private readonly IHost _host;

        public IdentityProviderApplication()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(Config.IdentityProviderUrl);
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