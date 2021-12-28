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
    public class AuthorController : Controller
    {
        private PustokContext _context;
        public AuthorController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Authors.Count())/ 4);
            ViewBag.SelectedPage = page;
            return View(_context.Authors.Skip((page-1)*4).Take(8).Include(x => x.Books).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);
            if (author==null)
            {
                return NotFound();
            }
            return View(author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Author excistauthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (excistauthor==null)
            {
                return NotFound();
            }

            excistauthor.FullName = author.FullName;

            _context.SaveChanges();

            return RedirectToAction("index", "author");
        }
        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);
            if (author ==  null)
            {
                return NotFound();
            }
            return View(author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Author excistauthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (excistauthor == null)
            {
                return NotFound();
            }

            excistauthor.FullName = author.FullName;
            _context.SaveChanges();
            return RedirectToAction("index", "author");
        }
    }
}
