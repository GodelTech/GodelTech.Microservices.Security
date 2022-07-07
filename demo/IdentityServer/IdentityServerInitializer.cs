using GodelTech.Microservices.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    /// <summary>
    /// This class is expected to be used for demo purposes only. More details can be found here:
    /// https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/tree/main
    /// https://identityserver4.readthedocs.io/en/latest/quickstarts/2_interactive_aspnetcore.html
    /// </summary>
    public class IdentityServerInitializer : IMicroserviceInitializer
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResource)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(Config.Users);

            builder.AddDeveloperSigningCredential();
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIdentityServer();
        }
    }
}
