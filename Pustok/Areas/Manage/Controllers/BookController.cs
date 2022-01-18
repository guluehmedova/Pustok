using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Helper;
using Pustok.Models;
using Pustok.Services;
using Pustok.ViewModels;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<PustokHub> _hubContext;
        public BookController(PustokContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IHubContext<PustokHub> hubContext)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _hubContext = hubContext;
        }
        public IActionResult Index( int page = 1, string search=null,bool? status=null)
        {
            var books = _context.Books.Include(x => x.Author).Include(x => x.bookComments).Include(x => x.Genre).Include(x => x.NewBookImages).Skip((page - 1) * 4).Take(8).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                books = books.Where(x => x.Name.ToUpper().Contains(search.ToUpper()));

            if (status != null)
                books = books.Where(x => x.IsDeleted == status);

            HomeViewModelArea homeViewModelArea = new HomeViewModelArea
            {
                PagenatedBooks = PagenatedList<Book>.Create(books, page, 2),
                Books = books.ToList(),
            };
            return View(homeViewModelArea);
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
               // path = Path.Combine(_env.WebRootPath, "uploads/teachers", book.ImageFiles);  //tapa bilmedim 
                //using (FileStream stream = new FileStream(path, FileMode.Create))
                //{
                //    book.ImageFile.CopyTo(stream);
                //}
                //excistbookx.Image = book.ImageFile.FileName;
            }

            _context.SaveChanges();
            return RedirectToAction("index", "teacher");
        }
        public IActionResult Delete(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book == null) { return NotFound(); }

            book.IsDeleted = true;
            _context.Books.Remove(book);
            _context.SaveChanges();

            return Ok();
        }
        public IActionResult Comments(int bookid)
        {
            List<BookComment> comments = _context.BookComments.Include(x=>x.Book).Where(x => x.Book.Id == bookid).ToList();
            return View(comments);
        }
        public IActionResult DeleteComment(int id)
        {
            BookComment comment = _context.BookComments.FirstOrDefault(x => x.Id == id);
            if (comment == null) return NotFound();

            _context.BookComments.Remove(comment);
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult InfoComment(int id)
        {
            BookComment comment = _context.BookComments.Include(x=>x.Book).FirstOrDefault(x => x.Id == id);
            if (comment == null) return NotFound();

            return View(comment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptComment(int id)
        {
            BookComment comment =  _context.BookComments.Include(x => x.Book).FirstOrDefault(x => x.Id == id);
            if (comment == null) return NotFound();
            comment.Status = true;
            _context.SaveChanges();

            //AppUser user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == comment.AppUserId);
            //if (user.ConnectionId != null)
            //{
            //    await _hubContext.Clients.Client(user.ConnectionId).SendAsync("CommentAccepted");
            //}
            return RedirectToAction("index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcceptItem(int id)
        {
            BookItem item = _context.BookItems.Include(x => x.Book).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            item.Status = true;

            _context.SaveChanges();

            return RedirectToAction("index");
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
            _emailService.Send(order.AppUser.Email, "Sifaris", order.CodeNumber + order.CodePrefix);

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
    }
}
