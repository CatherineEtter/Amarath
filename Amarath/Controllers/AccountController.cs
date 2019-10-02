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
        //UserManager is injected into the constructor for ASP.NET Identity
        public AccountController(UserManager<IdentityUser> userManager)
        {

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

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            return View();
        }

        public ActionResult LoginUser()
        {
            return RedirectToAction("Index");
        }
    }
}