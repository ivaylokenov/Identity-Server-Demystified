namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(config => 
                {
                    // We check the cookie to confirm that we are authenticated.
                    config.DefaultAuthenticateScheme = "ClientCookie";

                    // When we sign in, we will issue a cookie.
                    config.DefaultSignInScheme = "ClientCookie";

                    // We will use this to check if we are allowed to do something.
                    config.DefaultChallengeScheme = "OurServer";
                })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config => {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:5011/oauth/authorize";
                    config.TokenEndpoint = "https://localhost:5011/oauth/token";
                    config.SaveTokens = true; // Saves the token into the cookie.
                    
                    config.Events = new OAuthEvents
                    {
                        // This event populates the claims from the access token.
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var base64Payload = accessToken.Split('.')[1];
                            var bytes = Convert.FromBase64String(base64Payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach(var (type, value) in claims)
                            {
                                context.Identity.AddClaim(new Claim(type, value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();

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
