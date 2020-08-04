namespace IdentityServer
{
    using IdentityModel;
    using IdentityServer4.Models;
    using System.Collections.Generic;

    public static class Configuration
    {
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

                    // No interactive user, uses the ClientId & Secret for authentication.
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // Scopes that this client has access to.
                    AllowedScopes = { "cats" }
                }
            };
    }
}
