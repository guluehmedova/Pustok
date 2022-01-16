using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    //[Authorize(Roles ="SuperAdmin, Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public DashboardController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        UserName = "SuperAdmin",
        //        Fullname = "Super Admin"
        //    };
        //    var result = await _userManager.CreateAsync(appUser, "Admin123");
        //    return Ok(result);
        //}
    }
}
