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

    public class FeatureController : Controller
    {
        private readonly DataContext _dataContext;
        public FeatureController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Feature> sliders = _dataContext.Features.ToList();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Feature feature)
        {
            if (!ModelState.IsValid) return View();

            _dataContext.Features.Add(feature);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            Feature slider = _dataContext.Features.Find(id);

            if (slider is null) return View("Error");
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Feature feature)
        {
            Feature existfeature = _dataContext.Features.Find(feature.Id);

            if (existfeature is null) return View("Error");

            PropertyInfo[] destination = existfeature.GetType().GetProperties();
            PropertyInfo[] souce = feature.GetType().GetProperties();

            for (int i = 0; i < destination.Length; i++)
            {
                destination[i].SetValue(existfeature, souce[i].GetValue(feature));
            }


            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Feature deletefeature = _dataContext.Features.Find(id);

            _dataContext.Features.Remove(deletefeature);

            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
