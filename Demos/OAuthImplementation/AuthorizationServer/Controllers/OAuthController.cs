namespace AuthorizationServer.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    // Following the specification - https://tools.ietf.org/html/rfc6749#page-24
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // Authorization flow type - "code" for Authorization Code Grant.
            string client_id, // The client ID.
            string redirect_uri, // Redirect URI.
            string scope, // What information I want for the user.
            string state) // Random string generated to confirm that we are going to back to the same client.
        {
            // Validate the client here.

            var query = new QueryBuilder
            {
                {"redirectUri", redirect_uri},
                {"state", state}
            };

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string password,
            string redirectUri,
            string state)
        {
            // Validate the username and password against the database.
            // With ASP.NET Core Identity, for example.
            // Then generate an authorization code.

            const string authorizationCode = "AUTHCODE";

            var query = new QueryBuilder
            {
                {"code", authorizationCode},
                {"state", state}
            };

            return Redirect($"{redirectUri}{query}");
        }

        public IActionResult Token(
            string grant_type, // Defines what kind of grant was used for the token request.
            string code, // Authorization code which confirms the authentication process.
            string redirect_uri,  // Redirect URI.
            string client_id) // The client ID.
        {
            // The authorization code and the client should be validated here.

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "CodeItUp User ID"),
                new Claim(ClaimTypes.Name, "Ivaylo Kenov"),
                new Claim("CodeItUp", "Is Cool")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token = accessToken,
                token_type = "Bearer",
                some_claim = "Code It Up"
            };

            return this.Ok(responseObject);
        }

        [Authorize]
        public IActionResult Validate()
        {
            if (HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
            {
                // The access code should be validated here.

                return Ok();
            }

            return BadRequest();
        }
    }
}
