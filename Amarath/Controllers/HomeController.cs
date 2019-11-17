using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amarath.Models;
using Amarath.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Amarath.DAL.Data;

namespace Amarath.Controllers
{
    public class HomeController : Controller
    {
        private static DbContextOptionsBuilder<AmarathContext> optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
        private static readonly AmarathContext db = new AmarathContext(optionsBuilder.Options);

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public async Task<IActionResult> Codex(string sortOrder)
        {
            ViewData["ItemData"] = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            var items = from i in db.Items select i;

            switch(sortOrder)
            {
                case "name":
                    items.OrderBy(i => i.Name);
                    break;
                default:
                    items.OrderBy(i => i.Name);
                    break;
            }
            return View(await items.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
