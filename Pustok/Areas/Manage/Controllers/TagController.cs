using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class TagController : Controller
    {
        private PustokContext _context;
        public TagController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var tags = _context.Tags.Skip((page - 1) * 8).Take(8).AsQueryable();
            HomeViewModelArea homeareaVM = new HomeViewModelArea
            {
                Tags = tags.ToList(),
                PagenatedTags=PagenatedList<Tag>.Create(tags,page,2)
            };
            return View(homeareaVM);
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
