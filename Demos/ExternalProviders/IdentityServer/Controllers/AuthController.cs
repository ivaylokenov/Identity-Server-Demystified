namespace IdentityServer.Controllers
{
    using System.Security.Claims;
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
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await this.signInManager
                .GetExternalAuthenticationSchemesAsync();
            
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders
            });
        }

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

        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = this.Url
                .Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            
            var properties = this.signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUri);
            
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await this.signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var username = info.Principal
                .FindFirst(ClaimTypes.Name.Replace(" ", "_")).Value;

            return View("ExternalRegister", new ExternalRegisterViewModel
            {
                UserName = username,
                ReturnUrl = returnUrl
            });
        }

        public async Task<IActionResult> ExternalRegister(ExternalRegisterViewModel model)
        {
            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var user = new CodeItUpUser
            {
                UserName = model.UserName
            };

            var result = await this.userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return View(model);
            }

            result = await this.userManager.AddLoginAsync(user, info);

            if (!result.Succeeded)
            {
                return View(model);
            }

            await this.signInManager.SignInAsync(user, false);

            return Redirect(model.ReturnUrl);
        }
    }
}
