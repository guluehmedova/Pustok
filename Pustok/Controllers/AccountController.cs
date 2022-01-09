using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
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
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,PustokContext pustokContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _pustokContext = pustokContext;
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

            AppUser user = await _userManager.FindByEmailAsync(memberLoginVM.Username);
            if (user==null)
            { ModelState.AddModelError("Email", "Email Or Password Are Not True"); return View(); }

            var result = await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password, false, false);

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
            MemberProfilViewModel memberProfilViewModel = new MemberProfilViewModel
            {
                UserName = user.UserName,
                FullName=user.Fullname,
                Email=user.Email
            };
            return View(memberProfilViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(MemberProfilViewModel memberProfilVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(memberProfilVM.NewPassword) && !string.IsNullOrWhiteSpace(memberProfilVM.ConfirmNewPassword))
            {
                var passwordresult = await _userManager.ChangePasswordAsync(user, memberProfilVM.CurrentPassword, memberProfilVM.NewPassword);

                if (!passwordresult.Succeeded)
                {
                    foreach (var error in passwordresult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
            }

            if (user.Email != memberProfilVM.Email && _userManager.Users.Any(x=>x.NormalizedEmail==memberProfilVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "This Email Is Already Exsist!");
                return View();
            }

            user.UserName = memberProfilVM.UserName;
            user.Fullname = memberProfilVM.FullName;
            user.Email = memberProfilVM.Email;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("index", "home");
        }
    }
}
