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
            var items = from i in db.Items select i;

            switch(sortOrder)
            {
                case "name":
                    items = items.OrderBy(i => i.Name);
                    break;
                case "type":
                    items = items.OrderBy(i => i.Type);
                    break;
                case "intelligence":
                    items = items.OrderByDescending(i => i.Intelligence);
                    break;
                case "dexterity":
                    items = items.OrderByDescending(i => i.Dexterity);
                    break;
                case "strength":
                    items = items.OrderByDescending(i => i.Strength);
                    break;
                case "damage":
                    items = items.OrderByDescending(i => i.Damage);
                    break;
                case "defense":
                    items = items.OrderByDescending(i => i.Defense);
                    break;
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }
            return View(await items.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Bestiary(string sortOrder)
        {
            var enemies = from x in db.Enemies select x;

            switch (sortOrder)
            {
                case "name":
                    enemies = enemies.OrderBy(i => i.Name);
                    break;
                case "rank":
                    enemies = enemies.OrderByDescending(i => i.Rank);
                    break;
                case "health":
                    enemies = enemies.OrderByDescending(i => i.Health);
                    break;
                case "mindamage":
                    enemies = enemies.OrderByDescending(i => i.MinDamage);
                    break;
                case "maxdamage":
                    enemies = enemies.OrderByDescending(i => i.MaxDamage);
                    break;
                default:
                    enemies = enemies.OrderByDescending(i => i.Rank);
                    break;
            }
            return View(await enemies.AsNoTracking().ToListAsync());
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
