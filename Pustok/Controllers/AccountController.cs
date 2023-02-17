using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Data;
using Pustok.Models;
using Pustok.viewModel;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,DataContext dataContext,RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberRegister)
        {
            if(!ModelState.IsValid) return View();

            AppUser user = null;

            user = _dataContext.Users.FirstOrDefault(x => x.NormalizedUserName == memberRegister.Username.ToUpper());

            //user =await _userManager.FindByNameAsync(memberRegister.Username);

            if (user!= null)
            {
                ModelState.AddModelError("Username", "Already exist!");
                return View();
            }

            //user = await _userManager.FindByEmailAsync(memberRegister.Email);

            user = _dataContext.Users.FirstOrDefault(x => x.NormalizedEmail == memberRegister.Email.ToUpper());


            if (user != null)
            {
                ModelState.AddModelError("Email", "Already exist!");
                return View();
            }


            user = new AppUser
            {
                FullName = memberRegister.FullName,
                UserName = memberRegister.Username,
                Email = memberRegister.Email,
                IsAdmin=false
            };

            var result = await _userManager.CreateAsync(user, memberRegister.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            var result1 = await _userManager.AddToRoleAsync(user, "Member");


            _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("index","home");
        }
        public async Task<IActionResult> Profile()
        {
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }

            List<Order> Orders = _dataContext.Orders.Where(x => x.AppUserId == member.Id).ToList();


            return View(Orders);
        }

    }
}
