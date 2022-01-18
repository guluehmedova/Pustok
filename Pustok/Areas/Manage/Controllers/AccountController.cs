using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using Pustok.Services;
using Pustok.ViewModels;
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
        private readonly IEmailService _emailService;
        public AccountController(PustokContext context,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
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
            //AppUser user = _userManager.FindByNameAsync("SuperAdmin").Result;
            //var result = _userManager.AddToRoleAsync(user, "SuperAdmin").Result;
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
        public IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(ForgotPasswordViewModel forgotVM) //this name mean is - this action for area
        {
            if (!ModelState.IsValid) { return View(); }

            AppUser user = await _userManager.FindByEmailAsync(forgotVM.Email);
            if (user == null) { ModelState.AddModelError("Email", "This Email is not exist"); return View(); }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("resetpassword", "account", new { email = user.Email, token = token }, Request.Scheme);
            _emailService.Send(user.Email, url, "Reset Link");
            return Ok(new { url });
        }
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM)
        {
            AppUser user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
            if (user == null || !(await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordVM.Token)))
                return RedirectToAction("login");
            return View(resetPasswordVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel resetPasswordVM)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordVM.Password) || resetPasswordVM.Password.Length > 25)
                ModelState.AddModelError("Password", "Password is required and must be less than 26 character");

            if (!ModelState.IsValid) return View("ResetPassword", resetPasswordVM);

            AppUser user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
            if (user == null) return RedirectToAction("login");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("ResetPassword", resetPasswordVM);
            }

            TempData["Success"] = "Sifreniz ugurla yenilendi!";

            return RedirectToAction("login");
        }
    }
}
