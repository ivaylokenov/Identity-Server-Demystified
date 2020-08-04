namespace WebClient.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        public IActionResult Logout() => SignOut("WebClientCookie", "oidc");

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
    }
}
