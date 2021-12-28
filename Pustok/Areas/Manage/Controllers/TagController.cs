using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    public class TagController : Controller
    {
        private PustokContext _context;
        public TagController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Tags.Count()) / 8);
            ViewBag.SelectedPage = page;
            return View(_context.Tags.Skip((page - 1) * 8).Take(8).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction("index", "tag");
        }
        public IActionResult Edit(int id)
        {
            Tag tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag excisttag = _context.Tags.FirstOrDefault(x => x.Id == tag.Id);
            if (excisttag == null)
            {
                return NotFound();
            }

            excisttag.Name = tag.Name;

            _context.SaveChanges();

            return RedirectToAction("index", "tag");
        }
        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Tag tag)
        {
            Tag excisttag = _context.Tags.FirstOrDefault(x => x.Id == tag.Id);
            if (excisttag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(excisttag);
            _context.SaveChanges();

            return RedirectToAction("index", "tag");
        }
    }
}
