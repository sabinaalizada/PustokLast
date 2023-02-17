using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Data;
using Pustok.Models;
using Pustok.viewModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        public readonly DataContext _dataContext;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(DataContext dataContext,UserManager<AppUser> userManager)
        {
            _dataContext= dataContext;
            _userManager = userManager;
        }
        
        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Sliders = _dataContext.Sliders.OrderBy(x=>x.Order).ToList(),
                Features=_dataContext.Features.ToList(),
                FeaturedBooks=_dataContext.Books.Include(x=>x.Author).Include(x=>x.bookImages).Where(x=>x.DisCountPrice>0).ToList(),
                NewBooks=_dataContext.Books.Include(x=>x.Author).Include(x=>x.bookImages).Where(x=>x.DisCountPrice>0).ToList(),
                DiscountBooks=_dataContext.Books.Include(x=>x.Author).Include(x=>x.bookImages).Where(x=>x.DisCountPrice>0).ToList()

            };
            return View(homeViewModel);
        }

        public IActionResult SetSession(int id)
        {
            HttpContext.Session.SetString("UserId",id.ToString());  
            return Content("Added session");
        }

        public IActionResult GetSession()
        {
            string Id = HttpContext.Session.GetString("UserId");

            return Content(Id);
        }

        public IActionResult RemoveSession()
        {
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Index");   
        }

        public IActionResult SetCookie(int id)
        {
            List<int> Ids = new List<int>();

            string idsStr = HttpContext.Request.Cookies["BookIds"];

            if (idsStr != null)
            {
                Ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
                Ids.Add(id);
            }
            else
            {
                Ids.Add(id);
            }



            idsStr = JsonConvert.SerializeObject(Ids);

            HttpContext.Response.Cookies.Append("BookIds", idsStr);
            return Content("Added cookie");
        }

        public IActionResult GetCookie()
        {

            List<int> Ids = new List<int>();

            string IdsStr = HttpContext.Request.Cookies["BookIds"];

            if(IdsStr is not null)
            {
                Ids=JsonConvert.DeserializeObject<List<int>>(IdsStr);
            }

            //string name = HttpContext.Request.Cookies["UserName"];
            return Json(Ids);
        }

        public IActionResult GetBasketItems()
        {
            List<BasketItemViewModel> BasketItems = new List<BasketItemViewModel>();

            string basketItemStr = HttpContext.Request.Cookies["basketItems"];

            if (basketItemStr != null)
            {
                BasketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
            }

            return Json(BasketItems);
        }


        public async Task<IActionResult> AddToBasket(int bookid)
        {
            var result = _dataContext.Books.Find(bookid);

            if (bookid != result.Id)
            {
                return NotFound();
            }

            List<BasketItemViewModel> BasketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItemView = null;
            AppUser member = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }

            string basketItemStr = HttpContext.Request.Cookies["basketItems"];


            if (member == null)
            {


                if (basketItemStr is not null)
                {
                    BasketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);

                    basketItemView = BasketItems.FirstOrDefault(x => x.Bookid == bookid );

                   
                    
                        if (basketItemView is not null) basketItemView.Count++;
                        else
                        {
                            basketItemView = new BasketItemViewModel
                            {
                                Bookid = bookid,
                                Count = 1
                            };
                            BasketItems.Add(basketItemView);
                        }
                    

                }
                else
                {
                    basketItemView = new BasketItemViewModel
                    {
                        Bookid = bookid,
                        Count = 1
                    };

                    BasketItems.Add(basketItemView);
                }
                basketItemStr = JsonConvert.SerializeObject(BasketItems);

                HttpContext.Response.Cookies.Append("basketItems", basketItemStr);
            }
            else
            {
                BasketItem memberBasketItem = _dataContext.BasketItems.FirstOrDefault(x => x.AppUserId == member.Id && x.BookId == bookid );

                memberBasketItem.IsDeleted=false;

                if(memberBasketItem != null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        AppUserId = member.Id,
                        BookId = bookid,
                        Count = 1
                    };

                    _dataContext.BasketItems.Add(memberBasketItem);
                }
                _dataContext.SaveChanges();

            }
            return Ok();
        }


        public async Task<IActionResult> CheckOut()
        {
            AppUser member = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckOutItemViewModel> checkOutItems = new List<CheckOutItemViewModel>();
            CheckOutItemViewModel checkOutItem = null;
            List<BasketItem> memberBasketItem = null;
            OrderViewModel orderViewModel = null;
            string basketItemStr= HttpContext.Request.Cookies["basketItems"];

            if (member == null)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    foreach (var item in basketItems)
                    {
                        checkOutItem = new CheckOutItemViewModel
                        {
                            Book = _dataContext.Books.Include(x => x.bookImages).FirstOrDefault(x => x.Id == item.Bookid ),
                            Count = item.Count
                        };
                        checkOutItems.Add(checkOutItem);
                    }
                }
            }
            else
            {
                memberBasketItem = _dataContext.BasketItems.Include(x=>x.Book).Include(x=>x.Book.bookImages).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in memberBasketItem)
                {
                    if (!item.IsDeleted)
                    {
                        checkOutItem = new CheckOutItemViewModel
                        {
                            Book = item.Book,
                            Count = item.Count
                        };
                        checkOutItems.Add(checkOutItem);
                    }
                }
            }
            orderViewModel = new OrderViewModel
            {
                CheckOutItemViewModels= checkOutItems,
                FullName=member?.Id,
                Email=member?.Email,
                Phone=member?.PhoneNumber
            };
            return View(orderViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(OrderViewModel orderVM)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckOutItemViewModel> checkOutItems = new List<CheckOutItemViewModel>();
            CheckOutItemViewModel checkOutItem = null;
            List<BasketItem> memberBasketItem = null;
            OrderItem orderItem = null;
            double totalprice = 0;

            string basketItemStr = HttpContext.Request.Cookies["basketItems"];

            if (!ModelState.IsValid) return NotFound();
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }

            Order order = null;

            order = new Order
            {
                FullName = orderVM.FullName,
                Country = orderVM.Country,
                Address = orderVM.Address,
                City = orderVM.City,
                Email = orderVM.Email,
                Note = orderVM.Note,
                Phone = orderVM.Phone,
                ZipCode = orderVM.ZipCode,
                OrderStatus=Enums.OrderStatus.Pending,
                dateTime=DateTime.UtcNow.AddHours(4),
                AppUserId = member?.Id
            };

            if (member == null)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    foreach (var item in basketItems)
                    {
                        
                        Book book = _dataContext.Books.FirstOrDefault(x => x.Id == item.Bookid && x.BookCount > 0);
                        book.BookCount-=item.Count;
                     
                        orderItem = new OrderItem
                        {
                            Book = book,
                            BookName = book.Name,
                            CostPrice = book.CostPrice,
                            DiscountPrice = book.DisCountPrice,
                            SalePrice = (book.SalePrice * (1 - (book.DisCountPrice / 100))),
                            Count = item.Count,
                            Order = order
                        };

                        
                        totalprice += orderItem.SalePrice * orderItem.Count;
                        order.OrderItems.Add(orderItem);
                        item.Count = 0;
                    }

                    if (Request.Cookies["basketItems"] != null)
                    {
                        Response.Cookies.Delete("basketItems");
                    }
                }
            }
            else
            {
                memberBasketItem = _dataContext.BasketItems.Include(x => x.Book).Include(x => x.Book.OrderItems).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in memberBasketItem)
                {
                    Book book = _dataContext.Books.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == item.BookId );
                    book.BookCount -= item.Count; 
                    item.IsDeleted = true;
                    orderItem = new OrderItem
                    {
                        Book = book,
                        BookName = book.Name,
                        CostPrice = book.CostPrice,
                        DiscountPrice = book.DisCountPrice,
                        SalePrice = (book.SalePrice * (1 - (book.DisCountPrice / 100))),
                        Count = item.Count,
                        Order = order
                    };
                    totalprice += orderItem.SalePrice * orderItem.Count;
                    order.OrderItems.Add(orderItem);
                    item.Count = 0;
                }
            }
            order.TotalPrice = totalprice;
            

            _dataContext.Orders.Add(order);

            _dataContext.SaveChanges();
            //return Ok(orderVM);
            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> RemoveFromBasket(int bookid)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckOutItemViewModel> checkOutItems = new List<CheckOutItemViewModel>();
            List<BasketItem> memberBasketItems = null;
            BasketItem memberBasketItem = null;
            List<BasketItemViewModel> BasketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItemView = null;

            string basketItemStr = HttpContext.Request.Cookies["basketItems"];

            if (!ModelState.IsValid) return NotFound();
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            var result = _dataContext.Books.Find(bookid);

            if (bookid != result.Id)
            {
                return NotFound();
            }


            if (member == null)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);

                    if (basketItemStr is not null)
                    {
                        BasketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);

                        basketItemView = BasketItems.FirstOrDefault(x => x.Bookid == bookid);

                        if (basketItemView is not null)
                        {
                            if (basketItemView.Count > 1)
                            {
                                basketItemView.Count--;
                            }
                            else if (basketItemView.Count == 1)
                            {
                                BasketItems.Remove(basketItemView);
                            }
                        }
                    }
                    basketItemStr = JsonConvert.SerializeObject(BasketItems);

                    HttpContext.Response.Cookies.Append("basketItems", basketItemStr);

                }
            }
            else
            {
                memberBasketItems = _dataContext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();
                memberBasketItem = memberBasketItems.FirstOrDefault(x => x.BookId == bookid);

                if (memberBasketItem is not null)
                {
                    if (memberBasketItem.Count > 1)
                    {
                        memberBasketItem.Count--;
                    }
                    else if (memberBasketItem.Count == 1)
                    {
                        _dataContext.Remove(memberBasketItem);
                    }
                }
            }
            

            _dataContext.SaveChanges();
     
            return Ok();

        }


       

    }
}