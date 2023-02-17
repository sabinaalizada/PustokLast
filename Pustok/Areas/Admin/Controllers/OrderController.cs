using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Pustok.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Pustok.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(DataContext dataContext,IEmailSender emailSender,UserManager<AppUser> userManager)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
             List<Order> orders= _dataContext.Orders.ToList();


            return View(orders);
        }

        public IActionResult Detail(int id)
        {
            Order order = _dataContext.Orders.Include(x => x.OrderItems).OrderBy(x=>x.OrderStatus==Enums.OrderStatus.Pending).FirstOrDefault(x=>x.Id==id);
            



            if (order == null) return View("Error");


            return View(order);
        }

        public async Task<IActionResult> Accept(int id)
        {
            Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return View("Error");

            order.OrderStatus = Enums.OrderStatus.Accepted;

            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }

            _dataContext.SaveChanges();

            return RedirectToAction("index");  
        }

        public async Task<IActionResult> Reject(int id)
        {
            Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return View("Error");
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            

            order.OrderStatus = Enums.OrderStatus.Rejected;

            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
