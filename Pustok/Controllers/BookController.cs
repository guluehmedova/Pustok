using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class BookController:Controller
    {
        private PustokContext _context;
        public BookController(PustokContext pustokContext)
        {
            _context = pustokContext;
        }
        public IActionResult AddBook(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book ==null)
            {
                return NotFound();
            }
            List<BasketItemViewModel> basketitems = new List<BasketItemViewModel>();
            string IsBasketItemExcist = HttpContext.Request.Cookies["basketitemlist"];

            if(IsBasketItemExcist!=null)
            {
                basketitems = JsonConvert.DeserializeObject < List < BasketItemViewModel >> (IsBasketItemExcist);
            }

            BasketItemViewModel item = basketitems.FirstOrDefault(x => x.BookId == id);

            if(item == null)
            {
                item = new BasketItemViewModel
                {
                    BookId = id,
                    Count = 1
                };
                basketitems.Add(item);
            }
            else
            {
                item.Count++;
            }

            var bookidstr = JsonConvert.SerializeObject(basketitems);
            HttpContext.Response.Cookies.Append("basketitemlist", bookidstr);

            return Ok();
        }
        public IActionResult ShowBook()
        {
            var bookidstr = HttpContext.Request.Cookies["basketitemlist"];
            List<BasketItemViewModel> bookids = new List<BasketItemViewModel>();

            if(bookidstr != null)
            {
                bookids = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(bookidstr);
            }

            return Ok();
        }

        public IActionResult CheckOut()
        {
            List<CheckOutViewModel> checkoutitems = new List<CheckOutViewModel>();

            string basketitemsstr = HttpContext.Request.Cookies["basketitemlist"];

            if (basketitemsstr!=null)
            {
                List<BasketItemViewModel> basketitems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketitemsstr);

                foreach (var item in basketitems)
                {
                    CheckOutViewModel checkOutitem = new CheckOutViewModel
                    {
                        Book = _context.Books.FirstOrDefault(x => x.Id == item.BookId),
                        Count = item.Count
                    };

                    checkoutitems.Add(checkOutitem);
                }
            }
            return View(checkoutitems);
        }
    }
}
