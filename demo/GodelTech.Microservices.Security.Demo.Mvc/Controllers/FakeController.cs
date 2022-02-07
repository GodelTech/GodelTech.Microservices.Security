using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Security.Demo.Mvc.Controllers
{
    public class FakeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FakeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> A()
        {
            var client = _httpClientFactory.CreateClient("user_client");
            client.BaseAddress = new Uri("https://localhost:44301");

            ViewBag.Response = await client.GetStringAsync("fakes/1");

            return View("CallApi");
        }
    }
}
