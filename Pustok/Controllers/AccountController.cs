using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using Pustok.Services;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly PustokContext _pustokContext;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,PustokContext pustokContext, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _pustokContext = pustokContext;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterViewModel)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(memberRegisterViewModel.UserName);
            //eger bele bir user varsa demeli problem var cunki eyni userden iki dene ola bilmez
            if (user!=null) { ModelState.AddModelError("UserName", "UserName already exist"); return View(); }
           
            if (_pustokContext.Users.Any(x=>x.NormalizedEmail == memberRegisterViewModel.Email.ToUpper())) 
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

            return RedirectToAction("index", "home");
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(MemberLoginViewModel memberLoginVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == memberLoginVM.Username.ToUpper() && !x.IsAdmin);
            if (user==null)
            { ModelState.AddModelError("Email", "Email Or Password Are Not True"); return View(); }

            var result = await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password,  memberLoginVM.IsPersistent, false);

            if(!result.Succeeded) { ModelState.AddModelError("", "Email Or Password Are Not True"); return View();}

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profil()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            ProfileViewModel profileViewModel = new ProfileViewModel
            {
                Member = new MemberProfilViewModel
                {
                    UserName = user.UserName,
                    FullName = user.Fullname,
                    Email = user.Email
                },
                Orders = _pustokContext.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Book).Where(x => x.AppUserId == user.Id).ToList()
            };
            return View(profileViewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(MemberProfilViewModel memberProfilVM)
        {
            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);
            ProfileViewModel profilVM = new ProfileViewModel
            {
                Member = memberProfilVM
            };
            if(!ModelState.IsValid)
            {
                return View(profilVM);
            }

            if (member.Email != memberProfilVM.Email && _userManager.Users.Any(x => x.NormalizedEmail == memberProfilVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "This Email has laready been taken");
                return View(profilVM);
            }
            if (member.UserName!=memberProfilVM.UserName && _userManager.Users.Any(x=>x.NormalizedEmail==memberProfilVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "This Email has laready been taken");
                return View(profilVM);
            }

            member.Email = memberProfilVM.Email;
            member.Fullname = memberProfilVM.FullName;
            member.UserName = memberProfilVM.UserName;

            var result = await _userManager.UpdateAsync(member);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(profilVM);
            }

            if (!string.IsNullOrEmpty(memberProfilVM.CurrentPassword) && !string.IsNullOrEmpty(memberProfilVM.ConfirmNewPassword))
            {
                if (memberProfilVM.CurrentPassword != memberProfilVM.ConfirmNewPassword)
                {
                    return View(profilVM);
                }

                if (!await _userManager.CheckPasswordAsync(member, memberProfilVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword is not correct");
                    return View(profilVM);
                }

                var passwordResult = await _userManager.ChangePasswordAsync(member, memberProfilVM.CurrentPassword, memberProfilVM.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(profilVM);
                }
            }

            _pustokContext.SaveChanges();

            await _signInManager.SignInAsync(member, true);

            return View(profilVM);
           
        }
        public IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(ForgotPasswordViewModel forgotVM)
        {
            if(!ModelState.IsValid) { return View(); }

            AppUser user =await _userManager.FindByEmailAsync(forgotVM.Email);
            if(user==null) { ModelState.AddModelError("Email", "This Email is not exist"); return View(); }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("resetpassword", "account", new { email = user.Email, token = token }, Request.Scheme);
            _emailService.Send(user.Email,url,"Reset Link");
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
            if (string.IsNullOrWhiteSpace(resetPasswordVM.Password) || resetPasswordVM.Password.Length>25)
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
