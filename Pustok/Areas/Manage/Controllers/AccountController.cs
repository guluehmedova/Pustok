using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly PustokContext _context;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(PustokContext context,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModelArea memberRegisterViewModel)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(memberRegisterViewModel.UserName);
            //eger bele bir user varsa demeli problem var cunki eyni userden iki dene ola bilmez
            if (user != null) { ModelState.AddModelError("UserName", "UserName already exist"); return View(); }

            if (_context.Users.Any(x => x.NormalizedEmail == memberRegisterViewModel.Email.ToUpper()))
            { ModelState.AddModelError("Email", "Email already exist"); return View(); }

            user = new AppUser //eger hersey okaydirse onda user yaradiriq burda
            {
                Email = memberRegisterViewModel.Email,
                Fullname = memberRegisterViewModel.FullName,
                UserName = memberRegisterViewModel.UserName
            };

            var result = await _userManager.CreateAsync(user, memberRegisterViewModel.Password);

            if (!result.Succeeded)//eger paswword sehvdirse, bize xeta mesaji verecek bu kodlar vasitesile
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            //eger result succededirse, onda usere role elave edirik
            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("index", "dashboard");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel adminLoginViewModel)
        {
            if (!ModelState.IsValid) return View();

            AppUser admin = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == adminLoginViewModel.Username.ToUpper() && x.IsAdmin == true);
            if (admin == null) { ModelState.AddModelError("", "Pasword or username not true"); return View(); }

            var result = await _signInManager.PasswordSignInAsync(admin, adminLoginViewModel.Password, false, false);

            if (!result.Succeeded) { ModelState.AddModelError("", "Password or Username are not correct!"); return View(); }

            return RedirectToAction("index", "dashboard");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }
        public async Task<IActionResult> CreateRole()
        {
            IdentityRole role1 = new IdentityRole("SuperAdmin");
            IdentityRole role2 = new IdentityRole("Admin");
            IdentityRole role3 = new IdentityRole("Member");

            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);
            await _roleManager.CreateAsync(role3);

            return Ok();
        }
    }
}
