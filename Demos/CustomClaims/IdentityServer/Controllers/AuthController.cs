namespace IdentityServer.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Data;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class AuthController : Controller
    {
        private readonly SignInManager<CodeItUpUser> signInManager;
        private readonly UserManager<CodeItUpUser> userManager;
        private readonly IIdentityServerInteractionService interactionService;

        public AuthController(
            UserManager<CodeItUpUser> userManager,
            SignInManager<CodeItUpUser> signInManager, 
            IIdentityServerInteractionService interactionService)
        {
            this.signInManager = signInManager;
            this.interactionService = interactionService;
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
                await this.userManager
                    .AddClaimAsync(user, new Claim("CodeItUp.Car", model.CoolCar));

                await this.signInManager.SignInAsync(user, false);

                return Redirect(model.ReturnUrl);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await this.signInManager.SignOutAsync();

            var logoutRequest = await this.interactionService.GetLogoutContextAsync(logoutId);

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}
