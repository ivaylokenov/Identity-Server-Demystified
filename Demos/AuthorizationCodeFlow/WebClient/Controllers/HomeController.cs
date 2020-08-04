namespace WebClient.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();
    }
}
