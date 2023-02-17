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

    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Category> categories = _dataContext.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View();

            _dataContext.Categories.Add(category);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            Category category = _dataContext.Categories.Find(id);

            if (category is null) return View("Error");
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            Category existcategory = _dataContext.Categories.Find(category.Id);

            if (existcategory is null) return View("Error");

            PropertyInfo[] destination = existcategory.GetType().GetProperties();
            PropertyInfo[] souce = category.GetType().GetProperties();

            for (int i = 0; i < destination.Length; i++)
            {
                destination[i].SetValue(existcategory, souce[i].GetValue(category));
            }


            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Category deletecategory = _dataContext.Categories.Find(id);

            _dataContext.Categories.Remove(deletecategory);

            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
