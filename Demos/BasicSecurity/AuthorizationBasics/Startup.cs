namespace AuthorizationBasics
{
    using AuthorizationRequirements;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Security.Claims;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", config =>
                {
                    config.Cookie.Name = "CodeItUp.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            services
                .AddAuthorization(config =>
                {
                    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                    //var defaultAuthPolicy = defaultAuthBuilder
                    //    .RequireAuthenticatedUser()
                    //    .RequireClaim(ClaimTypes.DateOfBirth)
                    //    .Build();

                    // config.DefaultPolicy = defaultAuthPolicy;

                    // config.AddPolicy("CodeItUp", policyBuilder =>
                    // {
                    //     policyBuilder.RequireClaim("CodeItUp.Says");
                    // });

                    config.AddPolicy("Admin", policyBuilder =>
                    {
                        policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
                    });

                    config.AddPolicy("CodeItUp", policyBuilder =>
                    {
                        policyBuilder.RequireCustomClaim("CodeItUp.Says");
                    });
                });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Middleware stays the same.

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
