using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amarath.DAL.Models;
using Amarath.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Amarath.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<IdentityUserExt> userManager;

        // For coloring messages
        private string txtNormal = "#FFFFFF";
        private string txtDanger = "#FF0000";
        private string txtPlayer = "#FFFF00";
        private string txtOptions = "#66ffcc";

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

            List<KeyValuePair<string, string>> listDialog = null;
            List<KeyValuePair<string, string>> listAction = null;
            if (HttpContext.Session.GetString("Dialog") == null)
            {
                listDialog = new List<KeyValuePair<string, string>>();
                listAction = new List<KeyValuePair<string, string>>();
                listDialog.Add(new KeyValuePair<string, string>(location.Description, txtNormal));
                listAction.Add(new KeyValuePair<string, string>("You entered " + location.Name + "(Dungeon Level " + location.DungeonLevel + ")", txtNormal));
                HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
                HttpContext.Session.SetString("Action", JsonConvert.SerializeObject(listAction));
                HttpContext.Session.SetString("DungeonLevel", location.DungeonLevel.ToString());
            }

            GenerateOptions();
            return View();
        }

        public async Task<ActionResult> LevelUp()
        {
            //Increment player level
            var optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
            var db = new AmarathContext(optionsBuilder.Options);

            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            cChar.Rank += 1;
            db.SaveChanges();

            //Add Message to action list
            var listAction = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Action"));
            listAction.Add(new KeyValuePair<string, string>("You are now level " + cChar.Rank, txtPlayer));
            HttpContext.Session.SetString("Action", JsonConvert.SerializeObject(listAction));

            return RedirectToAction("Play", "Game");
        }
        public void GenerateOptions()
        {

            var rand = new Random();
            int randNum = rand.Next(1, 2);
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));

            if (Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel"))  == 0)
            {
                listDialog.Add(new KeyValuePair<string, string>(" - Proceed", txtOptions));
            }
            else if(randNum == 1)
            {
                listDialog.Add(new KeyValuePair<string, string>("You enter and look around.", txtNormal));
                listDialog.Add(new KeyValuePair<string, string>(" - Leave", txtOptions));
                listDialog.Add(new KeyValuePair<string, string>(" - Explore", txtOptions));
            } else
            {
                StartBattle();
            }
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
        }

        public ViewResult StartBattle()
        {
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));

            listDialog.Add(new KeyValuePair<string, string>("A monster appears!", txtDanger));
            listDialog.Add(new KeyValuePair<string, string>(" - Attack", txtOptions));
            listDialog.Add(new KeyValuePair<string, string>(" - Run", txtOptions));

            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));

            return View();
        }

        public ViewResult PlayerCommand()
        {

            return View();
        }
    }
}