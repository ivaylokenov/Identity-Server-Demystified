namespace Owners.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory) 
            => this.httpClientFactory = httpClientFactory;

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // Retrieve access token from Identity Server.

            var serverClient = this.httpClientFactory.CreateClient();

            var discoveryDocument = await serverClient
                .GetDiscoveryDocumentAsync("https://localhost:5021/");

            var tokenResponse = await serverClient
                .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,

                    ClientId = "OwnersAPI_ID",
                    ClientSecret = "OwnersAPI_Secret",
                    
                    Scope = "cats"
                });

            // Retrieve secret data by using the token.

            var apiClient = this.httpClientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:5023/Secret");

            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content
            });
        }
    }
}
