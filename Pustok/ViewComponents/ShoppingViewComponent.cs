using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Data;
using Pustok.Models;
using Pustok.viewModel;
using System;

namespace Pustok.ViewComponents
{
    public class ShoppingViewComponent : ViewComponent
    {
        public readonly DataContext _dataContext;
        private readonly UserManager<AppUser> _userManager;

        public ShoppingViewComponent(DataContext dataContext, UserManager<AppUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppUser member = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<ShoppingCardViewModel> shoppingCards = new List<ShoppingCardViewModel>();
            ShoppingCardViewModel shoppingCard = null;
            List<BasketItem> memberBasketItem = null;
            OrderViewModel orderViewModel = null;
            string basketItemStr = HttpContext.Request.Cookies["basketItems"];
            if (member == null)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    foreach (var item in basketItems)
                    {
                        shoppingCard = new ShoppingCardViewModel
                        {
                            Book = _dataContext.Books.Include(x => x.bookImages).FirstOrDefault(x => x.Id == item.Bookid),
                            Count = item.Count
                        };
                        shoppingCards.Add(shoppingCard);
                    }
                }
            }
            else
            {
                memberBasketItem = _dataContext.BasketItems.Include(x => x.Book).Include(x => x.Book.bookImages).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in memberBasketItem)
                {
                    if (!item.IsDeleted)
                    {
                        shoppingCard = new ShoppingCardViewModel
                        {
                            Book = item.Book,
                            Count = item.Count
                        };
                        shoppingCards.Add(shoppingCard);
                    }
                }
            }
            return View(shoppingCards);
        }
    }
}
