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

        private static DbContextOptionsBuilder<AmarathContext> optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
        private static readonly AmarathContext db = new AmarathContext(optionsBuilder.Options);

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
                listAction.Add(new KeyValuePair<string, string>("You entered " + location.Name + " (Dungeon Level " + location.DungeonLevel + ")", txtNormal));
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
            List<string> listChoices = new List<string>();
            //If character playing for the first time
            if (Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel"))  == 0)
            {
                listChoices.Add("proceed");
                listDialog.Add(new KeyValuePair<string, string>(" - Proceed", txtOptions));
            }
            else if(randNum == 1)
            {
                listChoices.Add("explore");
                listChoices.Add("proceed");
                listChoices.Add("leave");

                listDialog.Add(new KeyValuePair<string, string>("You enter and look around.", txtNormal));
                listDialog.Add(new KeyValuePair<string, string>(" - Explore", txtOptions));
                listDialog.Add(new KeyValuePair<string, string>(" - Proceed", txtOptions));
                listDialog.Add(new KeyValuePair<string, string>(" - Leave", txtOptions));
            }
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
            HttpContext.Session.SetString("Choices", JsonConvert.SerializeObject(listChoices));

            if (randNum == 0)
            {
                SpawnEnemies();
            }
        }

        public ViewResult SpawnEnemies()
        {
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));

            listDialog.Add(new KeyValuePair<string, string>("A monster appears!", txtDanger));
            listDialog.Add(new KeyValuePair<string, string>(" - Attack", txtOptions));
            listDialog.Add(new KeyValuePair<string, string>(" - Run", txtOptions));

            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));

            return View();
        }
        [HttpPost]
        public IActionResult PlayerCommand(PlayViewModel viewModel)
        {
            //TODO: Make the Choices list better
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));
            var listChoices = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("Choices"));
            
            Func<Task> task = null;
            // Execute appropriate action (if any)
            if (listChoices.Contains(viewModel.UserInput.ToLower()))
            {
                listDialog.Add(new KeyValuePair<string, string>(viewModel.UserInput, txtPlayer));
                switch (viewModel.UserInput.ToLower())
                {
                    case "proceed":
                        task = AscendLevel;
                        break;
                    case "leave":
                        task = DescendLevel;
                        break;
                    case "explore":
                        task = ExploreLevel;
                        break;
                    case "run":
                        task = DescendLevel;
                        break;
                    case "attack":
                        task = StartBattle;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                listDialog.Add(new KeyValuePair<string, string>(viewModel.UserInput + " is not a valid choice!", txtDanger));
            }
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
            //Call the action down here because the Session Dialog needs to be set so it doesn't get overwritten.
            if (task != null)
            {
                Task.Run(async () => { await task(); }).Wait();
            }

            viewModel.UserInput = ""; //Reset user input
            return View("Play", viewModel);
        }

        public async Task AscendLevel()
        {
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));
            var listAction = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Action"));
            //Increment dungeon level
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            cChar.DungeonLevel += 1;
            var location = db.Locations.First(x => x.DungeonLevel == cChar.DungeonLevel);
            db.SaveChanges();

            if (location.DungeonLevel == 1)
            {
                listDialog.Add(new KeyValuePair<string, string>("You wander up the stairs and open the door...", txtNormal));
            }
            listDialog.Add(new KeyValuePair<string, string>(location.Description, txtNormal));
            listAction.Add(new KeyValuePair<string, string>("You entered " + location.Name + " (Dungeon Level " + location.DungeonLevel + ")", txtNormal));

            //Save all the text
            HttpContext.Session.SetString("DungeonLevel", location.DungeonLevel.ToString());
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
            HttpContext.Session.SetString("Action", JsonConvert.SerializeObject(listAction));

            GenerateOptions();
        }
        public async Task DescendLevel()
        {
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));
            var listAction = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Action"));
            //Increment dungeon level
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            cChar.DungeonLevel -= 1;
            var location = db.Locations.First(x => x.DungeonLevel == cChar.DungeonLevel);
            db.SaveChanges();

            listDialog.Add(new KeyValuePair<string, string>(location.Description, txtNormal));
            listAction.Add(new KeyValuePair<string, string>("You entered " + location.Name + " (Dungeon Level " + location.DungeonLevel + ")", txtNormal));

            //Save all the text
            HttpContext.Session.SetString("DungeonLevel", location.DungeonLevel.ToString());
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
            HttpContext.Session.SetString("Action", JsonConvert.SerializeObject(listAction));

            GenerateOptions();
        }
        public async Task ExploreLevel()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
        }
        public async Task StartBattle()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
        }
    }
}