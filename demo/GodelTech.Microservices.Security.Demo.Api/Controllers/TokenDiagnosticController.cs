using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Security.Demo.Api.Controllers
{
    [Authorize]
    [Route("tokendiagnostics")]
    [ApiController]
    public class TokenDiagnosticController : ControllerBase
    {
        // https://nestenius.se/2023/06/02/debugging-jwtbearer-claim-problems-in-asp-net-core/
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            (string UserName, IEnumerable<Claim> Claims) result = (User.Identity?.Name ?? "Unknown Name", User.Claims);

            return new JsonResult(result);
        }

        [AllowAnonymous]
        [HttpGet("DefaultInboundClaimTypeMaps")]
        public IActionResult GetDefaultInboundClaimTypeMaps()
        {
            return Ok(JwtSecurityTokenHandler.DefaultInboundClaimTypeMap);
        }

        [AllowAnonymous]
        [HttpGet("DefaultOutboundClaimTypeMap")]
        public IActionResult GetDefaultOutboundClaimTypeMaps()
        {
            return Ok(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap);
        }
    }
}
