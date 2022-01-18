using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly PustokContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public OrderController(PustokContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index(int page = 1)
        {
            var orders = _context.Orders.Skip((page - 1) * 8).Take(8).ToList();
            //HomeViewModelArea homeareVM = new HomeViewModelArea
            //{
            //    Orders=orders.ToList(),
            //    PagenatedOrders = PagenatedList<Order>.Create(orders, page, 2)
            //};
            return View(orders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(Order order)
        {
            Order existorder = _context.Orders.FirstOrDefault(x => x.Id == order.Id);
            if (existorder == null) { return NotFound(); }
            existorder.Status = Enums.OrderStatus.Accepted;
            _context.SaveChanges();
            TempData["success"] = "successful";
            return RedirectToAction("index");
        }
        public async Task<IActionResult> RejectOrder(Order order)
        {
            Order existorder = _context.Orders.FirstOrDefault(x => x.Id == order.Id);
            if (existorder == null) return NotFound();
            existorder.Status = Enums.OrderStatus.Rejected;
            _context.Orders.Remove(existorder);
            _context.SaveChanges();
            return RedirectToAction("rejectcomment",existorder);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectComment()
        {
            return View();
        }
    }
}
