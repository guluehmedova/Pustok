using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private PustokContext _pustokContext;
        public HomeController(PustokContext pustokContext)
        {
            _pustokContext = pustokContext;
        }
        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Sliders = _pustokContext.Sliders.ToList(),
                Features = _pustokContext.Features.ToList(),
                Images=_pustokContext.Images.ToList(),
                PromotionOnes = _pustokContext.PromotionOnes.ToList(),
                FeaturedBooks = _pustokContext.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsFeatured).Take(20).ToList(),
                DiscountedBooks = _pustokContext.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.DiscountPercent > 0).Take(20).ToList(),
                NewBooks = _pustokContext.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsNew).Take(20).ToList()
            };
            return View(homeViewModel);
        }
        public IActionResult GetBook(int id)
        {
            Book book = _pustokContext.Books.Include(x => x.Genre).Include(x=>x.BookTags).ThenInclude(bt=>bt.Tag).Include(x => x.BookImages).FirstOrDefault(x => x.Id == id);
            return PartialView("_ModalBookDetail", book);
        }
        public IActionResult Detail(int id)
        {
            Book book = _pustokContext.Books.Include(x => x.Genre).Include(x => x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == id);
            return View(book);
        }
    }
}

