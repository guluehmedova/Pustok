using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.Services;
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
        private readonly IEmailService _emailService;
        public BookController(PustokContext pustokContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _context = pustokContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public IActionResult Index(int? genreId, double? min, double? max,int page=1)
        {
            var books = _context.Books.Include(x => x.Author).Include(x => x.NewBookImages).Include(x => x.Genre).AsQueryable();
            if (genreId != null)
                books = books.Where(x => x.GenreId == genreId);

            BookViewModel bookViewModel = new BookViewModel
            {
                Genres = _context.Genres.Skip((page - 1) * 8).Take(8).Include(x => x.Books).ToList(),
                Books=books.ToList(),
            };

            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Books.Count()) / 8);
            ViewBag.SelectedPage = page;

            //ViewBag.Min = books.Max(x=>x.SalePrice);
            //ViewBag.Max = books.Min(x => x.SalePrice);
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
            BasketViewModel data = null;
            AppUser user = null;
            //if (User.Identity.IsAuthenticated)
            //{
            //    user = await _userManager.FindByNameAsync(User.Identity.Name);
            //}
            if (user != null && user.IsAdmin==false)
            {
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(x => x.AppUserId == user.Id && x.BookId == bookId);
                if (basketItem ==null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        BookId = bookId,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }

                _context.SaveChanges();

                //data = _getBasketItems(_context.BasketItems.Include(x => x.Book).ThenInclude(x => x.NewBookImages).Where(x => x.AppUserId == user.Id).ToList());
            }
            else
            {
                List<CookieBasketItemViewModel> basketItems = new List<CookieBasketItemViewModel>();
                string existBasketItems = HttpContext.Request.Cookies["basketItemList"];

                if (existBasketItems != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(existBasketItems);
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

                data = _getBasketItems(basketItems);
            }

            return Ok(data);
        } //problem var
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
        public async Task<IActionResult> CheckOut()
        {
            CheckOutViewModel checkoutVM = new CheckOutViewModel
            {
                CheckoutItems = await _getCheckoutItems(),
                Order = new Order()
            };
            return View(checkoutVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(Order order)
        {
            AppUser user = null;
            List<CheckoutItemViewModel> checkoutItems = await _getCheckoutItems();

            if (User.Identity.IsAuthenticated)
            {
                user = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && x.IsAdmin == false);
            }

            if (checkoutItems.Count == 0)
                ModelState.AddModelError("", "There is not a any selected product");
            if (user == null && string.IsNullOrWhiteSpace(order.Email))
                ModelState.AddModelError("Email", "Email is required");
            if (user == null && string.IsNullOrWhiteSpace(order.FullName))
                ModelState.AddModelError("FullName", "FullName is required");

            if (!ModelState.IsValid)
            {
                return View("CheckOut", new CheckOutViewModel { CheckoutItems = checkoutItems, Order = order });
            }

            if (user != null)
            {
                order.Email = user.Email;
                order.FullName = user.Fullname;
                order.AppUserId = user.Id;
            }

            var lastOrder = _context.Orders.OrderByDescending(x => x.Id).FirstOrDefault();

            order.CodePrefix = order.FullName[0].ToString().ToUpper() + order.Email[0].ToString().ToUpper();
            order.CodeNumber = lastOrder == null ? 1001 : lastOrder.CodeNumber + 1;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.Status = Enums.OrderStatus.Pending;
            order.OrderItems = new List<OrderItem>();

            foreach (var item in checkoutItems)
            {
                OrderItem orderItem = new OrderItem
                {
                    BookId = item.Book.Id,
                    Count = item.Count,
                    CostPrice = item.Book.CostPrice,
                    SalePrice = item.Book.SalePrice,
                    DiscountPercent = item.Book.DiscountPercent
                };
                order.TotalAmount += orderItem.DiscountPercent > 0
                    ? (orderItem.SalePrice * (1 - orderItem.DiscountPercent / 100)) * orderItem.Count
                    : orderItem.SalePrice * orderItem.Count;

                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();
            _emailService.Send(order.AppUser.Email, "Sifaris", order.CodeNumber+order.CodePrefix);

            if (user != null)
            {
                _context.BasketItems.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == user.Id));
                _context.SaveChanges();
            }
            else
            {
                HttpContext.Response.Cookies.Delete("basketItemList");
            }
            return RedirectToAction("profile", "account");
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
        private BasketViewModel _getBasketItems(List<BasketItem> basketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };

            foreach (var item in basketItems)
            {
                BasketItemViewModel basketItem = new BasketItemViewModel
                {
                    Name = item.Book.Name,
                    Price = item.Book.DiscountPercent > 0 ? (item.Book.SalePrice * (1 - item.Book.DiscountPercent / 100)) : item.Book.SalePrice,
                    BookId = item.Book.Id,
                    Count = item.Count,
                    PosterImage = item.Book.NewBookImages.FirstOrDefault(x => x.PosterStatus == true)?.Image
                };

                basketItem.TotalPrice = basketItem.Count * basketItem.Price;
                basket.TotalAmount += basketItem.TotalPrice;
                basket.BasketItems.Add(basketItem);
            }
            return basket;
        }
        public IActionResult Detail(int id)
        {
            Book book = _context.Books
                .Include(x => x.NewBookImages).Include(x => x.Genre)
                .Include(x => x.BookTags).ThenInclude(x => x.Tag)
                .Include(x => x.Author).Include(x => x.bookComments)
                .FirstOrDefault(x => x.Id == id);

            if (book == null) return NotFound();
            BookDetailViewModel bookDetailViewModel = new BookDetailViewModel
            {
                Book = book,
                Comment= new BookComment(),
                RelatedBooks = _context.Books.
                Include(x => x.NewBookImages).Include(x => x.Author)
                .Where(x => x.GenreId == book.GenreId)
                .OrderByDescending(x => x.Id).Take(5).ToList()
            };
            return View(bookDetailViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Comment(BookComment comment)
        {
            Book book = _context.Books
              .Include(x => x.NewBookImages).Include(x => x.Genre)
              .Include(x => x.BookTags).ThenInclude(x => x.Tag)
              .Include(x => x.Author).Include(x => x.bookComments)
              .FirstOrDefault(x => x.Id == comment.BookId);

            if (book == null) return NotFound();

            BookDetailViewModel bookDetailViewModel = new BookDetailViewModel
            {
                Book = book,
                Comment = comment,
                RelatedBooks = _context.Books.Include(x => x.NewBookImages).Include(x => x.Author)
                .Where(x => x.GenreId == book.GenreId)
                .OrderByDescending(x => x.Id).Take(5).ToList()
            };

            if (!ModelState.IsValid)
            {
                TempData["error"] = "comment is not true";
                return View("Detail", bookDetailViewModel);
            }
            if (!_context.Books.Any(x => x.Id == comment.BookId))
            {
                TempData["error"] = "book not found";
                return View("Detail", bookDetailViewModel);
            }

            if (!User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(comment.Email))
                {
                    TempData["error"] = "email is required";
                    return View("Detail", bookDetailViewModel);
                }
                if (string.IsNullOrWhiteSpace(comment.FullName))
                {
                    TempData["error"] = "fullname is required";
                    return View("Detail", bookDetailViewModel);
                }
            }
            else
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                comment.AppUserId = user.Id;
                comment.FullName = user.Fullname;
                comment.Email = user.Email;
            }

            comment.Status = false;
            comment.CreateddAt = DateTime.UtcNow.AddHours(4);
            _context.BookComments.Add(comment);
            _context.SaveChanges();

            TempData["success"] = "successful";

            return RedirectToAction("detail", new { id = comment.BookId });
        }
        private async Task<List<CheckoutItemViewModel>> _getCheckoutItems()
        {
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();

            AppUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user != null && user.IsAdmin == false)
            {
                List<BasketItem> basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToList();

                foreach (var item in basketItems)
                {
                    CheckoutItemViewModel checkoutItem = new CheckoutItemViewModel
                    {
                        Book = item.Book,
                        Count = item.Count
                    };
                    checkoutItems.Add(checkoutItem);
                }
            }
            else
            {
                string basketItemsStr = HttpContext.Request.Cookies["basketItemList"];
                if (basketItemsStr != null)
                {
                    List<CookieBasketItemViewModel> basketItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItems)
                    {
                        CheckoutItemViewModel checkoutItem = new CheckoutItemViewModel
                        {
                            Book = _context.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }

            return checkoutItems;
        }
        public IActionResult GetBook(int id)
        {
            Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookTags).ThenInclude(bt => bt.Tag).Include(x => x.NewBookImages).FirstOrDefault(x => x.Id == id);
            return PartialView("_ModalBookDetail", book);
        }
        public IActionResult TrackOrder()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TrackOrder(OrderTrackCodeViewModel trackCode)
        {
            if (!ModelState.IsValid) return View();
            trackCode.Order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Book).FirstOrDefault(x => (x.CodePrefix + x.CodeNumber) == trackCode.Code);
            if (trackCode.Order == null)
            {
                ModelState.AddModelError("Code", "This Code Is Not Exist");
                return View();
            }
            return View(trackCode);
        }
    }
}
