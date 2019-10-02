using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ActionResult Add(int id=0)
        {
            User userModel = new User();
            return View(userModel);
        }

        /*[HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel viewModel)
        {
           if(ModelState.IsValid)
            {
                var user = new IdentityUserExt { UserName = viewModel.Username, Email = viewModel.EmailAddress, FirstName = viewModel.FirstName, LastName = viewModel.LastName};
                var result = await userManager.CreateAsync(user, viewModel.Password);
            }
            return View(viewModel); //for errors
        }*/

        public ActionResult LoginUser()
        {
            return RedirectToAction("Index");
        }
    }
}