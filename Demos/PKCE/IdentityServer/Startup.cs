namespace IdentityServer
{
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<CodeItUpDbContext>(config =>
                {
                    config.UseInMemoryDatabase("CodeItUp");
                });

            // AddIdentity registers user-related services.
            services
                .AddIdentity<CodeItUpUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<CodeItUpDbContext>()
                .AddDefaultTokenProviders();

            // Configures the Identity cookie.
            services
                .ConfigureApplicationCookie(config =>
                {
                    config.Cookie.Name = "CodeItUp.IdentityCookie";
                    config.LoginPath = "/Auth/Login";
                });

            services
                .AddIdentityServer()
                .AddAspNetIdentity<CodeItUpUser>()
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiScopes(Configuration.GetApiScopes())
                .AddInMemoryClients(Configuration.GetClients())
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // We use Identity server as a middleware here.
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
