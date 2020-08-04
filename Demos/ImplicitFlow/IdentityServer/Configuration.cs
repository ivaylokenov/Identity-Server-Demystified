namespace IdentityServer
{
    using IdentityModel;
    using IdentityServer4.Models;
    using System.Collections.Generic;
    using IdentityServer4;

    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("codeitup", new List<string>
                {
                    "CodeItUp.Car"
                })
            };

        public static IEnumerable<ApiScope> GetApiScopes()
            => new List<ApiScope>
            {
                new ApiScope("cats")
            };

        public static IEnumerable<Client> GetClients()
            => new List<Client> 
            {
                new Client 
                {
                    ClientId = "OwnersAPI_ID",
                    ClientSecrets = { new Secret("OwnersAPI_Secret".ToSha256()) },

                    // No interactive user, uses the ClientId & Secret for authentication & authorization.
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // Scopes that this client has access to.
                    AllowedScopes = { "cats" }
                },
                new Client
                {
                    ClientId = "WebClient_ID",
                    ClientSecrets = { new Secret("WebClient_Secret".ToSha256()) },

                    // Used Authorization Code Flow for authentication for authentication & authorization.
                    AllowedGrantTypes = GrantTypes.Code,

                    // Where to redirect to after login.
                    RedirectUris = { "https://localhost:5027/signin-oidc" },

                    // Where to redirect to after logout.
                    PostLogoutRedirectUris = { "https://localhost:5027/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        "cats",
                        "codeitup",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },

                    // Bigger ID token, one trip to Identity Server.
                    // AlwaysIncludeUserClaimsInIdToken = true,

                    // Allows refresh tokens.
                    AllowOfflineAccess = true,

                    // Disables the consent page - the one where the user agrees to share his data.
                    RequireConsent = false
                },
                new Client
                {
                    ClientId = "JavaScriptClient_ID",

                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = { "https://localhost:5025/home/signin" },
                    AllowedCorsOrigins = { "https://localhost:5025" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "cats"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false
                }
            };
    }
}
