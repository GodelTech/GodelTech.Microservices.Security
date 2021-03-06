using GodelTech.Microservices.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodeTech.Microservice.IdentityProviderWebsite
{
    /// <summary>
    /// This class is expected to be used for demo purposes only. More details can be found here:
    /// https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/tree/main
    /// https://identityserver4.readthedocs.io/en/latest/quickstarts/2_interactive_aspnetcore.html
    /// </summary>
    public class IdentityServerInitializer : MicroserviceInitializerBase
    {
        public IdentityServerInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer(x =>
                {
                    x.IssuerUri = "http://godeltech";
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(Config.Users);

            builder.AddDeveloperSigningCredential();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIdentityServer();
        }
    }
}