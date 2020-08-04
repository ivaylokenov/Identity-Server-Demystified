namespace WebClient
{
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

                    // Stores the tokens in the cookie after retrieval.
                    config.SaveTokens = true; 

                    config.ResponseType = "code";
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
