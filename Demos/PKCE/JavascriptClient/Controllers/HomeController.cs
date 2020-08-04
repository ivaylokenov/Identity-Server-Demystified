namespace JavascriptClient.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult SignIn() => View();
    }
}
