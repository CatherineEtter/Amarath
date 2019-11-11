using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amarath.DAL.Models;
using Amarath.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Amarath.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<IdentityUserExt> userManager;

        public GameController(UserManager<IdentityUserExt> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Play()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
            var db = new AmarathContext(optionsBuilder.Options);

            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var location = db.Locations.First(x => x.DungeonLevel == cChar.DungeonLevel);
            ViewBag.Dialog = location.Description;
            return View();
        }
        public async Task<ActionResult> LevelUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
            var db = new AmarathContext(optionsBuilder.Options);

            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            cChar.Rank += 1;

            db.SaveChanges();

            return RedirectToAction("Play", "Game");
        }
    }
}