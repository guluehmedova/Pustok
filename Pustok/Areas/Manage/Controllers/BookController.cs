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
    public class BookController : Controller
    {
        private PustokContext _context;
        public BookController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Books.Count()) / 4);
            ViewBag.SelectedPage = page;
            return View(_context.Books.Skip((page-1)*4).Take(8).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book ==  null)
            {
                return NotFound();
            }

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Book excistbook = _context.Books.FirstOrDefault(x => x.Id == book.Id);

            if (excistbook == null)
            {
                return NotFound();
            }

            excistbook.Name = book.Name;
            excistbook.Desc = book.Desc;
            excistbook.Code = book.Code;
            excistbook.CostPrice = book.CostPrice;
            excistbook.SalePrice = book.SalePrice;
            excistbook.DiscountPercent = book.DiscountPercent;
            excistbook.GenreId = book.GenreId;
            excistbook.AuthorId = book.AuthorId;

            _context.SaveChanges();

            return RedirectToAction("index", "book");
        }
        public IActionResult Delete(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book==null)
            {
                return NotFound();
            }

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Book excistbook = _context.Books.FirstOrDefault(x => x.Id == book.Id);

            if (excistbook == null)
            {
                return NotFound();
            }

            _context.Books.Remove(excistbook);
            _context.SaveChanges();

            return RedirectToAction("index", "book");
        }
    }
}
