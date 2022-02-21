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
            yield return new HstsInitializer();

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new ApiSecurityInitializer(
                options => Configuration.Bind("ApiSecurityOptions", options),
                new PolicyFactory()
            );

            yield return new ApiInitializer(Configuration);
        }
    }
```
PolicyFactory class:
```c#
    public class PolicyFactory : IAuthorizationPolicyFactory
    {
        public IReadOnlyDictionary<string, AuthorizationPolicy> Create()
        {
            return new Dictionary<string, AuthorizationPolicy>
            {
                ["add"] = GetAuthorizationPolicy("fake.add"),
                ["edit"] = GetAuthorizationPolicy("fake.edit"),
                ["delete"] = GetAuthorizationPolicy("fake.delete")
            };
        }

        private static AuthorizationPolicy GetAuthorizationPolicy(string requiredScope)
        {
            var policyBuilder = new AuthorizationPolicyBuilder();

            policyBuilder.RequireAuthenticatedUser();
            policyBuilder.RequireClaim("scope", requiredScope);

            return policyBuilder.Build();
        }
    }
```
Configuration file may have configuration section similar to this:

```json
  "ApiSecurityOptions": {
    "RequireHttpsMetadata": false,
    "Authority": "https://localhost:44300",
    "Issuer": "https://localhost:44300",
    "Audience": "DemoApi",
    "TokenValidation": {
      "ValidateAudience": true
    }
  }
```

**NOTE:** You controller or method must be decorated with `[Authorize]` attribute which has policy name specified. Here is how it may look like:

```c#
    [Authorize]
    [Route("fakes")]
    [ApiController]
    public class FakeController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IList<FakeModel>), StatusCodes.Status200OK)]
        public IActionResult GetList()
        {
            ...
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            ...
        }

        [Authorize("add")]
        [HttpPost]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status201Created)]
        public IActionResult Post([FromBody] FakePostModel model)
        {
            ...
        }
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
            yield return new DeveloperExceptionPageInitializer();
            yield return new ExceptionHandlerInitializer("/Home/Error");
            yield return new HstsInitializer();

            yield return new GenericInitializer(null, (app, _) => app.UseStaticFiles());

            yield return new GenericInitializer(null, (app, _) => app.UseRouting());

            yield return new UiSecurityInitializer(
                options => Configuration.Bind("UiSecurityOptions", options)
            );

            yield return new MvcInitializer();
        }
    }
```

The following configuration secription need to be added to `appsettings.json`:

```json
  "UiSecurityOptions": {
    "Authority": "https://localhost:44300",
    "Issuer": "https://localhost:44300",
    "ClientId": "Mvc",
    "ClientSecret": "secret",
    "RequireHttpsMetadata": false,
    "Scopes": [
      "openid",
      "profile",
      "offline_access",
      "api"
    ]
  }
```
**NOTE:** You need to decorate your MVC / Razor Pages controllers with `[Authorize]` attribute or apply corresponding conventions by creating subclass of `RazorPagesInitializer`.

## Configuration Options

`ApiSecurityInitializer` uses `ApiSecurityOptions` class. Full list of settings can be found in the following snippet:

```json
  "ApiSecurityOptions": {
    "RequireHttpsMetadata": false,
    "Authority": "https://localhost:44300",
    "Issuer": "https://localhost:44300",
    "Audience": "DemoApi",
    "TokenValidation": {
      "ValidateAudience": true,
      "ValidateIssuer": true,
      "ValidateIssuerSigningKey": true,
      "ValidateLifetime": true
    },
    "SaveToken": true,
    "IncludeErrorDetails": true
  }
```
**IMPORTANT:** By default all validation and security restrictions are turned ON.

`UiSecurityInitializer` uses `UiSecurityOptions` class. Full list of settings can be found in the following snippet:

```json
  "UiSecurityOptions": {
    "Authority": "https://localhost:44300",
    "Issuer": "https://localhost:44300",
    "ClientId": "Mvc",
    "ClientSecret": "secret",
    "GetClaimsFromUserInfoEndpoint": true,
    "RequireHttpsMetadata": false,
    "ResponseType": "code",
    "Scopes": [
      "openid",
      "profile",
      "offline_access",
      "api"
    ],
    "UsePkce": true,
    "PublicAuthorityUri": "https://localhost:44300",
    "SaveTokens": true
  }
```

**NOTE:** `PublicAuthorityUri` specific public address of identity provider. This setting might be useful when server-to-server communication happens within internal network (Kubernetes, docker-compose) but user uses public address to navigate to service.

## Links

The following resources might be useful to understand internals of current project:
* [IdentityServer Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/0_overview.html)
* [IdentityServer Examples](https://github.com/IdentityServer/IdentityServer4/tree/main/samples)
* [IdentityModel.AspNetCore](https://identitymodel.readthedocs.io/en/latest/aspnetcore/overview.html)