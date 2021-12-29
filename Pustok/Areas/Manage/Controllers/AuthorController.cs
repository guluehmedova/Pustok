using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private PustokContext _context;
        private IWebHostEnvironment _env;
        public AuthorController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            if (author.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image is required");
            }
            else if (author.ImageFile.FileName.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "Max size is 2MB");
            }
            else if (author.ImageFile.ContentType != "image/png" && author.ImageFile.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("ImageFile", "ContentType is not correct");
            }

            string filename = author.ImageFile.FileName.Length <= 64 ? author.ImageFile.FileName : (author.ImageFile.FileName.Substring(author.ImageFile.FileName.Length - 64, 64));
            filename = Guid.NewGuid().ToString() + filename;
            string path = Path.Combine(_env.WebRootPath, "uploads/authors", filename);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                author.ImageFile.CopyTo(stream);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            author.Image = filename;
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
