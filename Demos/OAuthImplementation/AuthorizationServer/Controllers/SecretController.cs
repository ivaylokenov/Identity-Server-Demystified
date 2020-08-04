namespace AuthorizationServer.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class SecretController : Controller
    {
        [Authorize]
        public string Index() => "Code It Up Authorization Server Secret";
    }
}
