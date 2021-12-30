using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Helper;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private PustokContext _context;
        private IWebHostEnvironment _env;
        public BookController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env; 
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Books.Count()) / 4);
            ViewBag.SelectedPage = page;
            return View(_context.Books.Include(x=>x.Author).Include(x=>x.Genre).Include(x=>x.NewBookImages).Skip((page-1)*4).Take(8).ToList());
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            if (!ModelState.IsValid) { return NotFound(); }

            if (!_context.Authors.Any(x=>x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author is not found");
                return View();
            }
            if (!_context.Genres.Any(x=>x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre is not found");
                return View();
            }

            if (book.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "PosterFile is required");
                return View();
            }
            else
            {
                if (book.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterFile", "Max size is 2MB");
                    return View();
                }
                else if (book.PosterFile.ContentType != "image/jpeg" && book.PosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterFile", "UnCorrect Image Format");
                    return View();
                }

                NewBookImage poster = new NewBookImage
                {
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterFile),
                    Book = book
                };

                _context.NewBookImages.Add(poster);
            }

            if (book.HoverPosterFile == null)
            {
                ModelState.AddModelError("HoverPosterFile", "PosterFile is required");
                return View();
            }
            else
            {
                if (book.HoverPosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverPosterFile", "Max size is 2MB");
                    return View();
                }
                else if (book.HoverPosterFile.ContentType != "image/jpeg" && book.HoverPosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterFile", "UnCorrect Image Format");
                    return View();
                }

                NewBookImage hoverfile = new NewBookImage
                {
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterFile),
                    Book = book,
                    PosterStatus = false
                };

                _context.NewBookImages.Add(hoverfile);
            }

            if (book.ImageFiles != null)
            {
                foreach (var item in book.ImageFiles)
                {
                    if (item.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "Max size is 2MB");
                        return View();
                    }
                    else if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "UnCorrect Image Format");
                        return View();
                    }
                    NewBookImage poster = new NewBookImage
                    {
                        Book=book,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item)
                    };
                    _context.NewBookImages.Add(poster);
                }
            }

            if (book.ImageFiles != null)
            {
                foreach (var item in book.ImageFiles)
                {
                    if (item.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "ImageFile max size is 2MB");
                        return View();
                    }
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "ContentType must be image/jpeg or image/png");
                        return View();
                    }


                    NewBookImage bookimage = new NewBookImage
                    {
                        Book = book,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item)
                    };

                    _context.NewBookImages.Add(bookimage);
                }
            }
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
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
           if(!ModelState.IsValid) { return View(); }
           Book excistbookx = _context.Books.FirstOrDefault(x => x.Id == book.Id);
           if(excistbookx==null) { return NotFound(); }

            foreach (var item in book.ImageFiles)
            {
                if (item.ToString().Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "ImageFile max size is 2MB");
                }
                if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFiles", "ContentType must be image/jpeg or image/png");
                }

                string filename = item.FileName.Length <= 64 ? item.FileName : (item.FileName.Substring(item.FileName.Length - 64, 64));
                filename = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "uploads/teachers", excistbookx.NewBookImages.FirstOrDefault(x=>x.PosterStatus==true)?.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                path = Path.Combine(_env.WebRootPath, "uploads/teachers", book.ImageFiles);  //tapa bilmedim 
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    book.ImageFile.CopyTo(stream);
                }
                excistbookx.Image = book.ImageFile.FileName;
            }

            _context.SaveChanges();
            return RedirectToAction("index", "teacher");
        }
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) { return NotFound(); }

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
