using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BosnaJezik.Models;
using BosnaJezik.Models.AccountViewModels;

namespace BosnaJezik.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;        
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            logger.LogDebug("Serving Login page");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["RetunrUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            logger.LogDebug("Invoking Login post..");
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(vm.Username, vm.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    logger.LogDebug($"User {vm.Username} signed in succeded");                    
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    logger.LogDebug($"User {vm.Username} signed in fail");
                    return View(vm);
                }
            }

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = vm.Username };
                var result = await userManager.CreateAsync(user, vm.Password);
                if(result.Succeeded)
                {
                    logger.LogInformation($"User {vm.Username} created");

                    await signInManager.SignInAsync(user, isPersistent:false);
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out");
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        [HttpGet]
        public IActionResult UserInfo()
        {
            ViewData["UserName"] = User.Identity.Name;
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}