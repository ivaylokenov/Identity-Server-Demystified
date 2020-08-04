namespace IdentityServer.Data
{
    using Microsoft.AspNetCore.Identity;

    public class CodeItUpUser : IdentityUser
    {
        public string CoolCar { get; set; }
    }
}
