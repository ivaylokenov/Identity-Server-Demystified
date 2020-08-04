namespace AuthorizationBasics.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        [Authorize(Policy = "CodeItUp")]
        public IActionResult SecretPolicy() => View("Secret");

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole() => View("Secret");

        public async Task<IActionResult> Authenticate()
        {
            var codeItUpClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Ivaylo"),
                new Claim(ClaimTypes.Email, "ivaylo@codeitup.today"),
                new Claim("CodeItUp.Says", "Top Lectures!"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var licenseClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Ivaylo Kenov"),
                new Claim("DrivingLicense", "Only for top cars!"),
            };

            var codeItUpIdentity = new ClaimsIdentity(codeItUpClaims, "Code It Up Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { codeItUpIdentity, licenseIdentity });

            await this.HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }
    }
}
