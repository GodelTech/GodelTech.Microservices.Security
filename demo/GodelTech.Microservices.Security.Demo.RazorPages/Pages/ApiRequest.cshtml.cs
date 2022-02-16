using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Security.Demo.RazorPages.Pages
{
    public class ApiRequestModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiRequestModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string ApiResponse { get; set; }

        public void OnGet()
        {
            throw new InvalidOperationException();
        }

        public async Task OnGetCallApiAsUserAsync()
        {
            var client = _httpClientFactory.CreateClient("UserClient");

            ApiResponse = await client.GetStringAsync("fakes/1");
        }

        public async Task OnGetCallApiAsClientAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            ApiResponse = await client.GetStringAsync("fakes/1");
        }
    }
}
