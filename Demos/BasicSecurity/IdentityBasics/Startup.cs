namespace IdentityBasics
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
                    config.LoginPath = "/Home/Login";
                });

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
