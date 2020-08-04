namespace WebClient.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory) 
            => this.httpClientFactory = httpClientFactory;

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        [Authorize]
        public async Task<IActionResult> Tokens()
        {
            var rawIdToken = await this.HttpContext.GetTokenAsync("id_token");
            var rawAccessToken = await this.HttpContext.GetTokenAsync("access_token");

            var idToken = new JwtSecurityTokenHandler().ReadJwtToken(rawIdToken);
            var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(rawAccessToken);

            return Json(new
            {
                UserClaims = this.User.Claims.Select(c => new { c.Type, c.Value }),
                IdTokenClaims = idToken.Claims.Select(c => new { c.Type, c.Value }),
                AccessTokenClaims = accessToken.Claims.Select(c => new { c.Type, c.Value })
            }, new JsonSerializerOptions
            { 
                WriteIndented = true
            });
        }

        [Authorize]
        public async Task<IActionResult> RefreshTokens()
        {
            var serverClient = this.httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:5021");

            // Refresh token is not a JWT.
            var refreshToken = await this.HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClient = this.httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient
                .RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    RefreshToken = refreshToken,
                    ClientId = "WebClient_ID",
                    ClientSecret = "WebClient_Secret"
                });

            var authInfo = await this.HttpContext.AuthenticateAsync("WebClientCookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await this.HttpContext.SignInAsync("WebClientCookie", authInfo.Principal, authInfo.Properties);

            return Ok();
        }
    }
}
