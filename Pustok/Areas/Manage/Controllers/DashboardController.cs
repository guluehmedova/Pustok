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
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class DashboardController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public DashboardController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new AppUser
            {
                UserName = "SuperAdmin",
                Fullname = "Super Admin",
                IsAdmin =true
            };
            var result = await _userManager.CreateAsync(appUser, "Admin123");
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
            return Ok(result);
        }
    }
}
