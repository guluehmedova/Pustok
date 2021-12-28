using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class FeatureController : Controller
    {
        private PustokContext _context;
        public FeatureController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Features.Count()) / 8);
            ViewBag.SelectedPage = page;
            return View(_context.Features.Skip((page - 1) * 8).Take(8).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Feature feature)
        {
            _context.Features.Add(feature);
            _context.SaveChanges();
            return RedirectToAction("index", "feature");
        }
        public IActionResult Edit(int id)
        {
            Feature feature = _context.Features.FirstOrDefault(x => x.Id == id);
            if (feature==null)
            {
                return NotFound();
            }
            return View(feature);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Feature excistfeature = _context.Features.FirstOrDefault(x => x.Id == feature.Id);
            if (excistfeature == null)
            {
                return NotFound();
            }

            excistfeature.Icon = feature.Icon;
            excistfeature.Text = feature.Text;
            excistfeature.Title = feature.Title;

            _context.SaveChanges();

            return RedirectToAction("index", "feature");
        }
        public IActionResult Delete(int id)
        {
            Feature feature = _context.Features.FirstOrDefault(x => x.Id == id);
            if (feature == null)
            {
                return NotFound();
            }
            return View(feature);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            Feature excistfeature = _context.Features.FirstOrDefault(x => x.Id == feature.Id);

            if (excistfeature == null)
            {
                return NotFound();
            }

            _context.Features.Remove(excistfeature);
            _context.SaveChanges();

            return RedirectToAction("index", "feature");
        }
    }
}
