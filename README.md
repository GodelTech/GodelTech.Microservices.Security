# GodelTech.Microservices.Security

## Overview

**GodelTech.Microservices.Security** contains initializer responsible for REST API endpoint security configuration.

## Quick Start

### REST API Security
In order to configure REST API security `Startup.cs` and application configuration files must be updated. `Startup` class must use `ApiSecurityInitializer`. The following snippet demonstrates one of possible options how to `Startup` may look like:

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration) 
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);
            yield return new GenericInitializer((app, env) => app.UseRouting());

            yield return new ApiSecurityInitializer(Configuration, new PolicyFactory());
            yield return new ApiInitializer(Configuration);
        }

        private class PolicyFactory : IAuthorizationPolicyFactory
        {
            public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
            {
                var policyBuilder = new AuthorizationPolicyBuilder();

                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireClaim("scope", "api1");

                return new Dictionary<string, AuthorizationPolicy>
                {
                    ["Weather API Policy"] =  policyBuilder.Build()
                };
            }
        }
    }
```
Configuration file may have configuration section similar to this:

```json
  "ApiSecurityConfig": {
    "AuthorityUri": "http://localhost:7777",
    "Issuer": "http://godeltech",
    "RequireHttpsMetadata": false
  },
```

**NOTE:** You controller must be decorated with `[Authorize]` attribute which has policy name specified. Here is how it may look like:

```c#
    [Authorize("Weather API Policy")]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        ...
    }
```

### UI Security

In order to configure UI security `Startup.cs` file needs to be updated. `UiSecurityInitializer` must be added to list of initializers. Here is example how your `Startup` class may look like:

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration)
            {
                ErrorHandlingPath = "/Error"
            };
            
            yield return new GenericInitializer((app, env) => app.UseStaticFiles());
            yield return new GenericInitializer((app, env) => app.UseRouting());
            
            yield return new UiSecurityInitializer(Configuration);
            yield return new RazorPagesInitializer(Configuration);
        }
    }
```

The following configuration secription need to be added to `appsettings.json`:

```json
  "UiSecurityConfig": {
    "AuthorityUri": "http://localhost:7777",
    "Issuer": "http://godeltech",
    "ClientId": "mvc",
    "ClientSecret": "secret",
    "Scopes": [
      "openid",
      "profile"
    ]
  }
```
**NOTE:** You need to decorate your pages \ MVC controllers with `[Authorize]` attribute or apply corresponding conventions by creating subclass of `RazorPagesInitializer`.


## Configuration Options

`ApiSecurityInitializer` is configured using configuration file section. Full list of settings can be found in the following snippet:

```json
  "ApiSecurityConfig": {
    "AuthorityUri": "http://localhost:7777",
    "Issuer": "http://godeltech",
    "RequireHttpsMetadata" : true,
    "TokenValidation": {
      "ValidateIssuer": false,
      "ValidateAudience": false,
      "ValidateLifetime": false,
      "ValidateIssuerSigningKey": false      
  } 
```
**IMPORTANT:** By default all validation and security restrictions are turned ON.

`UiSecurityInitializer` uses `UiSecurityConfig` configuration section to initialize security infrastructure. Here is full list of available properties:

```json
  "UiSecurityConfig": {
    "AuthorityUri": "http://localhost:7777",
    "PublicAuthorityUri": "http://my.domain.com",
    "ResponseType": "code",
    "Issuer": "http://godeltech",
    "ClientId": "mvc",
    "ClientSecret": "secret",
    "GetClaimsFromUserInfoEndpoint": true,
    "RequireHttpsMetadata": false,
    "Scopes": [
      "openid",
      "profile"
    ]
  }
```

**NOTE:** `PublicAuthorityUri` specific public address of identity provider. This setting might be useful when server-to-server communication happens within internal network (Kubernates, docker-compose) but user uses public address to navigate to service.

## Links

The following resources might be useful to understand internals of current project:
* [IdentityServer Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/0_overview.html)
* [IdentityServer Examples](https://github.com/IdentityServer/IdentityServer4/tree/main/samples)