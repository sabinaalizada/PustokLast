using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Admin.viewModels;
using Pustok.Models;
using Pustok.viewModel;

namespace Pustok.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel memberLoginView)
        {
            if (!ModelState.IsValid) return View();
            AppUser admin = await _userManager.FindByNameAsync(memberLoginView.UserName);
            if (admin == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(admin, memberLoginView.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            return RedirectToAction("Index", "home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }
    }
}
