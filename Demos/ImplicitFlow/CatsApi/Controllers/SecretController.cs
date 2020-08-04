namespace CatsApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class SecretController : Controller
    {
        [Authorize]
        [Route("/Secret")]
        public string Index() => "Code It Up Secret Cats";
    }
}
