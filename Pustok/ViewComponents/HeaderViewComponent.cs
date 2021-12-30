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

namespace Pustok.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {
        private PustokContext _context;
        public HeaderViewComponent(PustokContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            BasketViewModel basket = null;
            var basketItemStr = HttpContext.Request.Cookies["basketItemList"];
            if (basketItemStr !=null)
            {
                List<CookieBasketItemViewModel> cookieItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(basketItemStr);
                basket = _getBasketItems(cookieItems);
            }
            HeaderViewModel headerViewModel = new HeaderViewModel
            {
                Genres = await _context.Genres.ToListAsync(),
                Settings = await _context.Settings.ToListAsync(),
                Basket= basket
            };
            return View(headerViewModel);
        }

        private BasketViewModel _getBasketItems(List<CookieBasketItemViewModel> cookiebasketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };
            foreach (var item in cookiebasketItems)
            {
                Book book = _context.Books.Include(x => x.NewBookImages).FirstOrDefault(x => x.Id == item.BookId);
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
    }
}
