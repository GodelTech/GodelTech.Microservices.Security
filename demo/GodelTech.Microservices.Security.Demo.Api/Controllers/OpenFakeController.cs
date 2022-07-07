using System.Collections.Generic;
using GodelTech.Microservices.Security.Demo.Api.Models.OpenFake;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Security.Demo.Api.Controllers
{
    [AllowAnonymous]
    [Route("openFakes")]
    [ApiController]
    public class OpenFakeController : ControllerBase
    {
        private static readonly IReadOnlyList<OpenFakeModel> Items = new List<OpenFakeModel>
        {
            new OpenFakeModel(),
            new OpenFakeModel
            {
                Id = 1,
                Title = "Test Title"
            }
        };

        [HttpGet]
        [ProducesResponseType(typeof(IList<OpenFakeModel>), StatusCodes.Status200OK)]
        public IActionResult GetList()
        {
            return Ok(Items);
        }
    }
}
