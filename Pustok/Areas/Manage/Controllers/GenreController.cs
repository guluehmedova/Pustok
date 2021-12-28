using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private PustokContext _context;
        public GenreController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Genres.Count())/4);
            ViewBag.SelectedPage = page;
            return View(_context.Genres.Include(x=>x.Books).Skip((page-1)*4).Take(8).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("index","genre");
        }
        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
            if (genre ==null)
            {
                return NotFound();
            }
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Genre excistgenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);
            if (excistgenre == null)
            {
                return NotFound();
            }

            excistgenre.Name = genre.Name;

            _context.SaveChanges();

            return RedirectToAction("index", "genre");
        }
        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Genre genre)
        {
            Genre excistgenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);
            if (excistgenre==null)
            {
                return NotFound();
            }

            _context.Genres.Remove(excistgenre);
            _context.SaveChanges();

            return RedirectToAction("index", "genre");
        }
    }
}
