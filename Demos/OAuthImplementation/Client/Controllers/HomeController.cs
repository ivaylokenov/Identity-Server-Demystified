namespace Client.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly HttpClient client;

        public HomeController(IHttpClientFactory httpClientFactory) 
            => this.client = httpClientFactory.CreateClient();

        public IActionResult Index() => View();

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            this.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var authServerResponse = await this.client.GetAsync("https://localhost:5011/secret/index");

            var resourceResponse = await this.client.GetAsync("https://localhost:5013/secret/index");

            return View(model: $"Auth Server: {await authServerResponse.Content.ReadAsStringAsync()}, API: {await resourceResponse.Content.ReadAsStringAsync()}");
        }
    }
}
