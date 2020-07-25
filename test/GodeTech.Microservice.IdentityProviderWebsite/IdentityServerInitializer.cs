using GodelTech.Microservices.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodeTech.Microservice.IdentityProviderWebsite
{
    public class IdentityServerInitializer : MicroserviceInitializerBase
    {
        public IdentityServerInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer()
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients);

            builder.AddDeveloperSigningCredential();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIdentityServer();
        }
    }
}