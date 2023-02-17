using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Helpers;
using Pustok.Models;
using System.Data;
using System.IO;
using System.Reflection;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]

    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _dataContext;
        public BookController(DataContext dataContext, IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Book> books = _dataContext.Books.ToList();
            return View(books);
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            if (!ModelState.IsValid) return View();


            //book.ImageUrl = book.ImageFile.SaveFile(_env.WebRootPath, "uploads/books");

            if (book.PosterImageFile != null)
            {
                if (book.PosterImageFile.ContentType != "image/png" && book.PosterImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload png or jpeg");
                    return View();
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload image size lower then 2 Mb");
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = true

                };
                _dataContext.BookImages.Add(bookImage);
            }

            if (book.HoverImageFile != null)
            {
                if (book.HoverImageFile.ContentType != "image/png" && book.HoverImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload png or jpeg");
                    return View();
                }

                if (book.HoverImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload image size lower then 2 Mb");
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = false

                };
                _dataContext.BookImages.Add(bookImage);
            }

            if (book.ImageFiles != null)
            {
                foreach (IFormFile imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload png or jpeg");
                        return View();
                    }

                    if (imageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload image size lower then 2 Mb");
                        return View();
                    }

                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        ImageUrl = imageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster = null

                    };
                    _dataContext.BookImages.Add(bookImage);
                }
            }




            _dataContext.Books.Add(book);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            Book book = _dataContext.Books.Include(x=>x.bookImages).FirstOrDefault(x=>x.Id==id);

            if (book is null) View("Error");
            return View(book);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update( Book book)
        {
            Book existbook = _dataContext.Books.Include(x => x.bookImages).FirstOrDefault(x=>x.Id==book.Id);
            if (existbook is null) return View("Error");

            //_dataContext.BookImages.(x => !book.BookImageIds.Contains(x.Id) && x.IsPoster == null);
            
            
            

            if (book.ImageFiles != null)
            {
                foreach (IFormFile imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload png or jpeg");
                        return View();
                    }

                    if (imageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload image size lower then 2 Mb");
                        return View();
                    }
                    //existbook.bookImages.RemoveAll(x => !book.BookImageIds.Contains(x.Id) && x.IsPoster == null);

                    //foreach (var item in existbook.bookImages.FindAll(b => !book.BookImageIds.Contains(b.Id) && b.IsPoster == null))
                    //{
                    //    var folderPath = Path.Combine(_env.WebRootPath, "uploads/books", item.ImageUrl);
                    //    System.IO.DirectoryInfo folderInfo = new DirectoryInfo(folderPath);

                    //    foreach (FileInfo file in folderInfo.GetFiles())
                    //    {
                    //        file.Delete();
                    //    }
                    //}


                    BookImage bookImage = new BookImage
                    {
                        Book = existbook,
                        ImageUrl = imageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster = null

                    };
                    _dataContext.BookImages.Add(bookImage);
                }
            }

            if (book.HoverImageFile != null)
            {
                if (book.HoverImageFile.ContentType != "image/png" && book.HoverImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload png or jpeg");
                    return View();
                }

                if (book.HoverImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload image size lower then 2 Mb");
                    return View();
                }

                string deletepath = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books");
                string delete = Path.Combine(_env.WebRootPath, "uploads/books", deletepath);

                if (System.IO.File.Exists(delete))
                {
                    System.IO.File.Delete(delete);
                }

                BookImage bookImage = new BookImage
                {
                    Book = existbook,
                    ImageUrl = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = false

                };
                _dataContext.BookImages.Add(bookImage);
            }

            if (book.PosterImageFile != null)
            {
                if (book.PosterImageFile.ContentType != "image/png" && book.PosterImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload png or jpeg");
                    return View();
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload image size lower then 2 Mb");
                    return View();
                }


                string deletepath = Path.Combine(_env.WebRootPath, "uploads/sliders", book.PosterImageFile.Name);

                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }

                BookImage bookImage = new BookImage
                {
                    Book = existbook,
                    ImageUrl = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = true

                };
                _dataContext.BookImages.Add(bookImage);
            }

            //string filename = existbook.ImageUrl;

            //string path = "C:\\Users\\99450\\source\\repos\\Pustok\\Pustok\\wwwroot\\uploads\\books\\" + filename;

            //System.IO.File.Delete(path);

            //book.ImageUrl = book.ImageFile.SaveFile(_env.WebRootPath, "uploads/books");

            existbook.Author = book.Author;
            existbook.CostPrice = book.CostPrice;
            existbook.DisCountPrice = book.DisCountPrice;
            existbook.SalePrice = book.SalePrice;
            existbook.IsAvailable = book.IsAvailable;
            existbook.IsNew = book.IsNew;
            existbook.Name = book.Name;
            existbook.CategoryId = book.CategoryId;
            existbook.AuthorId = book.AuthorId;

            //PropertyInfo[] destination=existbook.GetType().GetProperties();
            //PropertyInfo[] souce=book.GetType().GetProperties();

            //for (int i = 0; i < destination.Length; i++)
            //{
            //    destination[i].SetValue(existbook, souce[i].GetValue(book));
            //}


            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Book deletebook = _dataContext.Books.Find(id);


            _dataContext.Books.Remove(deletebook);

            List<BookImage> bookImages = _dataContext.BookImages.Where(x => x.BookId == id).ToList();

            foreach (BookImage item in bookImages)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/books", item.ImageUrl);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            
            _dataContext.SaveChanges();

            return Ok();
        }


    }
}
