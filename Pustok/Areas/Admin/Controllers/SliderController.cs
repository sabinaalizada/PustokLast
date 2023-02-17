using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustok.Data;
using Pustok.Helpers;
using Pustok.Models;
using System.Data;
using System.Reflection;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]

    public class SliderController: Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _env;
        public SliderController(DataContext dataContext,IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env=env;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _dataContext.Sliders.ToList();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if(slider.ImageFile.ContentType!="image/png" && slider.ImageFile.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("ImageFile", "You can only upload png or jpeg");
                return View();
            }

            if (slider.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "You can only upload image size lower then 2 Mb");
                return View();

            }

            slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");

            if (!ModelState.IsValid) return View();

            _dataContext.Sliders.Add(slider);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            Slider slider = _dataContext.Sliders.Find(id);

            if (slider is null) return View("Error");
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Slider slider)
        {
            

            Slider existslider = _dataContext.Sliders.Find(slider.Id);

            if (existslider is null) return View("Error");


            if(slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/png" && slider.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "You can only upload png or jpeg");
                    return View();
                }

                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "You can only upload image size lower then 2 Mb");
                    return View();

                }


                string deletepath = Path.Combine(_env.WebRootPath, "uploads/sliders", existslider.ImageUrl);

                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
                if (!ModelState.IsValid) return View();


                existslider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
            }
            
            if(!ModelState.IsValid) return View("Error");
            //string filename = existslider.ImageUrl;

            //string path = "C:\\Users\\99450\\source\\repos\\Pustok\\Pustok\\wwwroot\\uploads\\sliders\\" + filename;

            //System.IO.File.Delete(path);

            //slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");

            existslider.Desc = slider.Desc;
            existslider.ButtonText=slider.ButtonText;
            existslider.Order=slider.Order;
            existslider.RedirectUrl = slider.RedirectUrl;
            existslider.Title1 = slider.Title1;
            existslider.Title2 = slider.Title2;

            //PropertyInfo[] destination = existslider.GetType().GetProperties();
            //PropertyInfo[] souce = slider.GetType().GetProperties();

            //for (int i = 0; i < destination.Length; i++)
            //{
            //    destination[i].SetValue(existslider, souce[i].GetValue(slider));
            //}

            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Slider deleteslide = _dataContext.Sliders.FirstOrDefault(x=>x.Id==id);

            if (deleteslide is null) return NotFound();


            _dataContext.Sliders.Remove(deleteslide);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\sliders", deleteslide.ImageUrl);

            System.IO.File.Delete(path);

            _dataContext.SaveChanges();

            return Ok();

            //return RedirectToAction(nameof(Index));
        }
    }
}
