using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustok.Data;
using Pustok.Models;
using System.Data;
using System.Reflection;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]

    public class AuthorController : Controller
    {

        private readonly DataContext _dataContext;
        public AuthorController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Author> authors = _dataContext.Authors.ToList();
            return View(authors);
        }

        public IActionResult Create()
        {
            return View();
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid) return View();

            _dataContext.Authors.Add(author);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            Author author = _dataContext.Authors.Find(id);

            if (author is null) return View("Error");
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Author author)
        {
            Author existauthor = _dataContext.Authors.Find(author.Id);

            if (existauthor is null) return View("Error");

            PropertyInfo[] destination = existauthor.GetType().GetProperties();
            PropertyInfo[] souce = author.GetType().GetProperties();

            for (int i = 0; i < destination.Length; i++)
            {
                destination[i].SetValue(existauthor, souce[i].GetValue(author));
            }


            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Author deleteauthor = _dataContext.Authors.Find(id);

            _dataContext.Authors.Remove(deleteauthor);

            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }    
}
