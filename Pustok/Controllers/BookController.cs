using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Pustok.viewModel;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        public readonly DataContext _dataContext;

        public BookController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            Book book=_dataContext.Books
                .Include(x=>x.Author)
                .Include(x => x.Category)
                .Include(x=>x.bookImages)
                .FirstOrDefault(x=>x.Id == id);
            if (book == null) return View("Error");

            BookViewModel bookVM = new BookViewModel
            {
                Book = book,
                Books = _dataContext.Books
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Include(x => x.bookImages)
                .Where(x=>x.DisCountPrice>0).ToList()
            };


            return View(bookVM);
        }
    }
}
