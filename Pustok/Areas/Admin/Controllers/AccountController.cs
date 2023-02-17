using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Admin.viewModels;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel adminLoginVM)
        {
            if (!ModelState.IsValid) return View(); 
            AppUser admin =await _userManager.FindByNameAsync(adminLoginVM.UserName);
            if (admin == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            var result=await _signInManager.PasswordSignInAsync(admin,adminLoginVM.Password,false,false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            return RedirectToAction("Index","Dashboard");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

    }
}
