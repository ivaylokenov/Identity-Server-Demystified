namespace WebClient
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(config => 
                {
                    config.DefaultScheme = "WebClientCookie";

                    // What scheme to use for authorization.
                    config.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("WebClientCookie")
                .AddOpenIdConnect("oidc", config => 
                {
                    config.Authority = "https://localhost:5021";

                    config.ClientId = "WebClient_ID";
                    config.ClientSecret = "WebClient_Secret";

                    config.SignedOutCallbackPath = "/Home/Index";

                    // Stores the tokens in the cookie after retrieval.
                    config.SaveTokens = true;

                    config.ResponseType = "code";

                    // Smaller ID token, two trips to Identity Server.
                    config.GetClaimsFromUserInfoEndpoint = true;

                    // Maps the user information to claims. You can also delete claims.
                    config.ClaimActions.MapUniqueJsonKey("car", "CodeItUp.Car");

                    // Adds custom claim scope. You can also remove the default scopes (note that "openid" is required).
                    config.Scope.Add("codeitup"); 
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
