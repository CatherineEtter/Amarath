using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.DAL.Data;
using Amarath.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amarath.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUserExt> userManager;
        private readonly SignInManager<IdentityUserExt> signInManager;

        //UserManager is injected into the constructor for ASP.NET Identity
        public AccountController(UserManager<IdentityUserExt> userManager, SignInManager<IdentityUserExt> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            ViewData["Message"] = "Login Page";
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Message"] = "Create an Account";
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel viewModel)
        {
            
            if(ModelState.IsValid)
            {
                var updatedUser = await userManager.GetUserAsync(User);
                var passConfirmResult = await signInManager.CheckPasswordSignInAsync(updatedUser, viewModel.Password, false);
                if(passConfirmResult.Succeeded)
                {
                    updatedUser.UserName = viewModel.Username;
                    updatedUser.FirstName = viewModel.FirstName;
                    updatedUser.LastName = viewModel.LastName;
                    updatedUser.Email = viewModel.EmailAddress;

                    var result = await userManager.UpdateAsync(updatedUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile", "Account");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Credentials");

            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, viewModel.RememberMe, false); 

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Credentials");
                }
            }

            return View(viewModel); //for errors
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel viewModel)
        {
           if(ModelState.IsValid)
            {
                var user = new IdentityUserExt { UserName = viewModel.Username, Email = viewModel.EmailAddress, FirstName = viewModel.FirstName, LastName = viewModel.LastName};
                var result = await userManager.CreateAsync(user, viewModel.Password);

                if(result.Succeeded)
                {
                    //TODO: Decide to use session or permanent cookie
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(viewModel); //for errors
        }
    }
}