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

        // For coloring messages
        private string normal = "#FFFFFF";
        private string danger = "#FF0000";
        private string player = "#FFFF00";
        private string options = "#66ffcc";

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
            /*
            ViewBag.Dialog = location.Description;
            ViewBag.Action = "You entered " + location.Name + " ( Dungeon Level " + location.DungeonLevel + " )";
            */

            //Session["Test"] = 9;
            ViewBag.DungeonLevel = location.DungeonLevel;
            ViewBag.Dialog = new List<KeyValuePair<string, string>>();
            ViewBag.Dialog.Add(new KeyValuePair<string, string>(location.Description, normal));

            ViewBag.Action = new List<KeyValuePair<string, string>>();
            ViewBag.Action.Add(new KeyValuePair<string, string>("You entered " + location.Name + "(Dungeon Level " + location.DungeonLevel + ")", normal));
            

            GenerateOptions();
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

            ViewBag.Actions.Add(new KeyValuePair<string, string>("You are now level " + cChar.Rank + "!", normal));

            return RedirectToAction("Play", "Game");
        }
        public void GenerateOptions()
        {

            var rand = new Random();
            int randNum = rand.Next(1, 2);
            if(ViewBag.DungeonLevel == 0)
            {
                ViewBag.Dialog.Add(new KeyValuePair<string, string>(" - Proceed", options));
            }
            else if(randNum == 1)
            {
                ViewBag.Dialog.Add(new KeyValuePair<string, string>("You enter and look around.", normal));
                ViewBag.Dialog.Add(new KeyValuePair<string, string>(" - Leave", options));
                ViewBag.Dialog.Add(new KeyValuePair<string, string>(" - Explore", options));
            } else
            {
                StartBattle();
            }

            
        }

        public ViewResult StartBattle()
        {
            ViewBag.Action.Add(new KeyValuePair<string, string>("A monster appears!", danger));
            ViewBag.Dialog.Add(new KeyValuePair<string, string>(" - Attack", options));
            ViewBag.Dialog.Add(new KeyValuePair<string, string>(" - Run", options));

            return View();
        }

        public ViewResult PlayerCommand()
        {
            ViewBag.Dialog.Add(new KeyValuePair<string, string>("Test", player));
            //ViewBag.Test = player;
            return View();
        }
    }
}