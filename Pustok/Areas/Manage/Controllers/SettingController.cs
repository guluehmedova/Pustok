using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SettingController : Controller
    {
        private PustokContext _context;
        public SettingController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Settings.Count()) / 8);
            ViewBag.SelectedPage = page;
            return View(_context.Settings.Skip((page - 1) * 8).Take(8).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Setting setting)
        {
            _context.Settings.Add(setting);
            _context.SaveChanges();
            return RedirectToAction("index", "setting");
        }
        public IActionResult Edit(int id)
        {
            Setting setting = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Setting excistsetting= _context.Settings.FirstOrDefault(x => x.Id == setting.Id);
            if (excistsetting == null)
            {
                return NotFound();
            }

            excistsetting.Key = setting.Key;
            excistsetting.Value = setting.Value;

            _context.SaveChanges();

            return RedirectToAction("index", "setting");
        }
        public IActionResult Delete(int id)
        {
            Setting setting = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            Setting excistsetting = _context.Settings.FirstOrDefault(x => x.Id == setting.Id);

            if (excistsetting == null)
            {
                return NotFound();
            }

            _context.Settings.Remove(excistsetting);
            _context.SaveChanges();

            return RedirectToAction("index", "setting");
        }
    }
}
