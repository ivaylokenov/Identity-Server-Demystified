namespace IdentityServer.Controllers
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class AuthController : Controller
    {
        private readonly SignInManager<CodeItUpUser> signInManager;
        private readonly UserManager<CodeItUpUser> userManager;

        public AuthController(
            UserManager<CodeItUpUser> userManager,
            SignInManager<CodeItUpUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Check if the model is valid.

            var result = await this.signInManager
                .PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                // Show an error message
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
            => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new CodeItUpUser
            {
                UserName = model.Username,
                CoolCar = model.CoolCar
            };

            var result = await this.userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await this.signInManager.SignInAsync(user, false);

                return Redirect(model.ReturnUrl);
            }

            return View();
        }
    }
}
