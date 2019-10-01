using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amarath.Controllers
{
    public class AccountController : Controller
    {
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
    }
}