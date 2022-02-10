using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Security.Demo.Mvc.Controllers
{
    public class ApiRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiRequestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> CallApiAsClient()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            ViewBag.Response = await client.GetStringAsync("fakes/1");

            return View("CallApi");
        }
    }
}
