namespace Resources.AuthRequirement
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    public class JwtRequirement : IAuthorizationRequirement { }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient client;
        private readonly HttpContext httpContext;

        public JwtRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            this.client = httpClientFactory.CreateClient();
            this.httpContext = httpContextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            JwtRequirement requirement)
        {
            if (this.httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ').Last();

                var response = await client
                    .GetAsync($"https://localhost:5011/oauth/validate?access_token={accessToken}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
