namespace IdentityBasics.Controllers
{
    using System;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Data;

    public class HomeController : Controller
    {
        private readonly UserManager<CodeItUpUser> userManager;
        private readonly SignInManager<CodeItUpUser> signInManager;
        
        public HomeController(
            UserManager<CodeItUpUser> userManager,
            SignInManager<CodeItUpUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user != null)
            {
                var signInResult = await this.signInManager
                    .PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Show an error message.
                    throw new InvalidOperationException("Invalid login.");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new CodeItUpUser
            {
                UserName = username,
                Email = "ivaylo@codeitup.today",
                CoolCar = "BMW 640D"
            };

            var result = await this.userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await this.userManager
                    .AddClaimAsync(user, new Claim("CodeItUp.Car", user.CoolCar));

                var signInResult = await this.signInManager
                    .PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Show an error message.
                    throw new InvalidOperationException("Invalid login.");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LogOut()
        {
            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index");
        }
    }
}
