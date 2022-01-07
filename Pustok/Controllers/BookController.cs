﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pustok.ViewModels.BasketViewModel;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private PustokContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public BookController(PustokContext pustokContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = pustokContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            BookViewModel bookViewModel = new BookViewModel
            {
                Genres = _context.Genres.Include(x => x.Books).ToList(),
                Books=_context.Books.Include(x=>x.Author).Include(x=>x.NewBookImages).Include(x=>x.Genre).ToList(),
            };
            return View(bookViewModel);
        }
        public IActionResult AddBasket(int bookId)
        {
            #region movcud olmayan bir kitab elave edile biler-ona gorede bu yoxlamani aparmaliyiq
            if (!_context.Books.Any(x => x.Id == bookId))
            {
                return NotFound();
            }
            #endregion
            List<CookieBasketItemViewModel> basketItems = new List<CookieBasketItemViewModel>();//bidene bos list yaradiriq
            string existBasketItems = HttpContext.Request.Cookies["basketItemList"];//cookiden gelen datani gotururuk
            if (existBasketItems != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(existBasketItems);
                //eger gelen data null-a beraber deyilse deserialize edib cookiebasketitemviewmodel cinsinden bir liste ceviririk
            }
            CookieBasketItemViewModel item = basketItems.FirstOrDefault(x => x.BookId == bookId);
            if (item == null)
            {
                item = new CookieBasketItemViewModel
                {
                    BookId = bookId,
                    Count = 1
                };
                basketItems.Add(item);
            }
            else
            {
                item.Count++;
            }
            var bookIdStr = JsonConvert.SerializeObject(basketItems);
            HttpContext.Response.Cookies.Append("basketItemList", bookIdStr);

            var data = _getBasketItems(basketItems);
            return Ok(data);
        }
        public IActionResult ShowBasket()
        {
            var bookIdStr = HttpContext.Request.Cookies["basketItemList"];
            List<CookieBasketItemViewModel> bookIds = new List<CookieBasketItemViewModel>();
            if (bookIdStr != null)
            {
                bookIds = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(bookIdStr);
            }
            return Json(bookIds);
        }
        public IActionResult GetBook(int id)
        {
            Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookTags).ThenInclude(bt => bt.Tag).Include(x => x.NewBookImages).FirstOrDefault(x => x.Id == id);
            return PartialView("_ModalBookDetail", book);
        }
        public IActionResult Detail(int id)
        {
            Book book = _context.Books.Include(x => x.Genre).Include(x => x.NewBookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);
            return View(book);
        }

        
        #region CheckOut sehifesi
        public IActionResult CheckOut()
        {
            List<CheckOutViewModel> checkoutItems = new List<CheckOutViewModel>();
            string basketItemsStr = HttpContext.Request.Cookies["basketItemList"];
            if (basketItemsStr != null)
            {
                List<CheckOutViewModel> basketItems = JsonConvert.DeserializeObject<List<CheckOutViewModel>>(basketItemsStr);
                //burda eger cookide itemler yoxdursa bos list qaytaracaq, yox eger varsa
                //hemin cookideki itemleri checkoutviewmodel listi duzeldib qaytarcaq
                foreach (var item in basketItems)
                {
                    CheckOutViewModel checkoutitem = new CheckOutViewModel
                    {
                        Book = _context.Books.FirstOrDefault(x => x.Id == item.Id),
                        Count = item.Count
                    };
                    checkoutItems.Add(checkoutitem);
                }
            }
            return View();
        }
        #endregion
        #region Kitablari sebete elave edende melumatlarin viewcarda ekave etmek hissesi
        private BasketViewModel _getBasketItems(List<CookieBasketItemViewModel> cookiebasketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };
            foreach (var item in cookiebasketItems) 
            {
                Book book = _context.Books.Include(x=>x.NewBookImages).FirstOrDefault(x => x.Id == item.BookId);
                BasketItemViewModel basketItem = new BasketItemViewModel
                {
                    Name = book.Name,
                    Price = book.DiscountPercent > 0 ? (book.SalePrice * (1 - book.DiscountPercent / 100)) : book.SalePrice,
                    BookId = book.Id,
                    Count = item.Count,
                    PosterImage = book.NewBookImages.FirstOrDefault(x => x.PosterStatus == true)?.Image,
                };

                basketItem.TotalPrice = basketItem.Count * basketItem.Price;
                basket.TotalAmount += basketItem.TotalPrice;
                basket.BasketItems.Add(basketItem);
            }

            return basket;
        }
        #endregion
      
    }
}
