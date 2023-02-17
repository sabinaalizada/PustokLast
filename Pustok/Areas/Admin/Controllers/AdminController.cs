using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Admin.viewModels;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _dataContext;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(UserManager<AppUser> userManager,DataContext dataContext,SignInManager<AppUser> signInManager )
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(AdminRegisterViewModel adminRegisterView)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = null;

            user = _dataContext.Users.FirstOrDefault(x => x.NormalizedUserName == adminRegisterView.Username.ToUpper());

            //user =await _userManager.FindByNameAsync(memberRegister.Username);

            if (user != null)
            {
                ModelState.AddModelError("Username", "Already exist!");
                return View();
            }

            //user = await _userManager.FindByEmailAsync(memberRegister.Email);

            user = _dataContext.Users.FirstOrDefault(x => x.NormalizedEmail == adminRegisterView.Email.ToUpper());


            if (user != null)
            {
                ModelState.AddModelError("Email", "Already exist!");
                return View();
            }


            user = new AppUser
            {
                FullName = adminRegisterView.FullName,
                UserName = adminRegisterView.Username,
                Email = adminRegisterView.Email,
            };

            var result = await _userManager.CreateAsync(user, adminRegisterView.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            var result1 = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Role is incorrect");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("login","account");
        }
    }
}
