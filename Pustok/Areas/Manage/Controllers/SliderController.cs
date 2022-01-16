using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private PustokContext _context;
        private IWebHostEnvironment _env;
        public SliderController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            var sliders = _context.Sliders.AsQueryable();
            HomeViewModelArea homeareVM = new HomeViewModelArea
            {
                Slider = sliders.ToList(),
               PagenatedSliders=PagenatedList<Slider>.Create(sliders,page,2)
            };
            return View(homeareVM);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (slider.Title1 == slider.Title2)
            {
                ModelState.AddModelError("title2", "Title2 can't be like Title1");
            }
            if (slider.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image is requried");
            }
            else if (slider.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "Max size is 2MB");
            }
            else if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "UnCorrect Image Format");
            }

            string filename = slider.ImageFile.FileName.Length <= 64 ? slider.ImageFile.FileName : (slider.ImageFile.FileName.Substring(slider.ImageFile.FileName.Length - 64,64));
            filename = Guid.NewGuid().ToString() + filename;
            string path = Path.Combine(_env.WebRootPath, "uploads/sliders", filename);

            using (FileStream stream = new FileStream(path,FileMode.Create))
            {
                slider.ImageFile.CopyTo(stream);
            }

            if (!ModelState.IsValid) {return View();}

            slider.Image = filename;
            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index", "slider");
        }
        public IActionResult Edit(int id)
        {
            if(!ModelState.IsValid) { return View(); }
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) {return NotFound();}
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) { return View(); }

            Slider excistslider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (excistslider == null) { return NotFound(); }

            string oldpath = Path.Combine(_env.WebRootPath, "uploads/sliders", excistslider.Image);

            if (System.IO.File.Exists(oldpath))
            {
                System.IO.File.Delete(oldpath);
            }

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Max size is 2MB");
                }
                else if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "UnCorrect Image Format");
                }

                string filename = slider.ImageFile.FileName.Length <= 64 ? slider.ImageFile.FileName : (slider.ImageFile.FileName.Substring(slider.ImageFile.FileName.Length - 64, 64));
                filename = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "uploads/sliders", filename);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.ImageFile.CopyTo(stream);
                }
                excistslider.Image = filename;
            }
            excistslider.Title1 = slider.Title1;
            excistslider.Title2 = slider.Title2;
            excistslider.Order = slider.Order;
            excistslider.RedirectUrl = slider.RedirectUrl;
            excistslider.BtnText = slider.BtnText;
            excistslider.Desc = slider.Desc;

            _context.SaveChanges();

            return RedirectToAction("index", "slider");
        }

        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if(slider == null) { return NotFound(); }

            if (!string.IsNullOrWhiteSpace(slider.Image))
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/sliders", slider.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            _context.Sliders.Remove(slider);
            _context.SaveChanges();

            return Ok();
        }
    }
}
