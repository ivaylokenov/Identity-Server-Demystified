namespace StatelessSecurity.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Secret()
        {
            var data = new
            {
                this.User.Identity.Name,
                Claims = this.User.Claims.Select(c => new
                {
                    c.Type,
                    c.Value
                })
            };

            return Ok(data);
        }

        public IActionResult Authenticate()
        {
            // You can use the UserManager from the ASP.NET Core Identity
            // to get all the necessary user data. The method you may use are:
            // - FindByEmailAsync or FindByNameAsync
            // - CheckPasswordAsync

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "CodeItUp User ID"),
                new Claim(ClaimTypes.Name, "Ivaylo Kenov"), 
                new Claim("CodeItUp", "Is Cool")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);

            var signingCredentials = new SigningCredentials(
                key, 
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            return Ok(new { access_token = handler.WriteToken(token) });
        }

        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);
            return Ok(Encoding.UTF8.GetString(bytes));
        }
    }
}
