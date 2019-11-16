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
                GenerateOptions();
            }
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
            Random rand = new Random();
            int randNum = rand.Next(0, 2); //50% chance to spawn a monster

            ClearChoices();
            //Don't spawn anything in level 0
            if (Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel"))  == 0)
            {
                AddToChoices("proceed");
                AddToDialog(" - Proceed", txtOptions);
            }
            else if(randNum == 1)
            {
                AddToChoices("explore");
                AddToChoices("proceed");
                AddToChoices("leave");
                AddToDialog("You enter and look around.", txtNormal);
                AddToDialog(" - Explore", txtOptions);
                AddToDialog(" - Proceed", txtOptions);
                AddToDialog(" - Leave", txtOptions);
            }

            if (randNum == 0)
            {
                SpawnEnemies();
            }
        }

        public ActionResult SpawnEnemies()
        {
            int dlevel = Int32.Parse(HttpContext.Session.GetString("DungeonLevel"));
            var cEnemy = db.Enemies.First(x => x.Rank == dlevel);

            AddToChoices("attack");
            AddToChoices("run");

            AddToAction("A " + cEnemy.Name + " appears!", txtDanger);
            AddToAction("Lvl: " + cEnemy.Rank + "| HP: " + cEnemy.Health + "| Min Dmg: " + cEnemy.MinDamage + "| Max Dmg: " + cEnemy.MaxDamage, txtDanger);
            AddToDialog(" - Attack", txtOptions);
            AddToDialog(" - Run", txtOptions);

            return RedirectToAction("Play", "Game");
        }
        [HttpPost]
        public IActionResult PlayerCommand(PlayViewModel viewModel)
        {
            //TODO: Make the Choices list better
            var listChoices = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("Choices"));
            
            Func<Task> task = null;
            // Execute appropriate action (if any)
            if (listChoices.Contains(viewModel.UserInput.ToLower()))
            {
                AddToDialog(viewModel.UserInput, txtPlayer);
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
                        AddToAction("You flee the fight!", txtNormal);
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
                AddToDialog(viewModel.UserInput + " is not a valid choice!", txtDanger);
            }
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
            //Increment dungeon level
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            if(cChar.DungeonLevel < 12)
            {
                cChar.DungeonLevel += 1;
                var location = db.Locations.First(x => x.DungeonLevel == cChar.DungeonLevel);
                db.SaveChanges();

                if (location.DungeonLevel == 1)
                {
                    AddToDialog("You wander up the stairs and open the door...", txtNormal);
                }
                AddToDialog(location.Description, txtNormal);
                AddToAction("You entered " + location.Name + " (Dungeon Level " + location.DungeonLevel + ")", txtNormal);

                //Save the Dungeon level
                HttpContext.Session.SetString("DungeonLevel", location.DungeonLevel.ToString());
                GenerateOptions();
            }
        }
        public async Task DescendLevel()
        {
            //Increment dungeon level
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            if(cChar.DungeonLevel > 1)
            {
                cChar.DungeonLevel -= 1;
                var location = db.Locations.First(x => x.DungeonLevel == cChar.DungeonLevel);
                db.SaveChanges();

                AddToDialog(location.Description, txtNormal);
                AddToAction("You entered " + location.Name + " (Dungeon Level " + location.DungeonLevel + ")", txtNormal);

                //Save all the text
                HttpContext.Session.SetString("DungeonLevel", location.DungeonLevel.ToString());

                GenerateOptions();
            }
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
        // =================== Methods to handle HTTP Session =================== //
        public void AddToDialog(string str, string txt)
        {
            var listDialog = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Dialog"));
            listDialog.Add(new KeyValuePair<string, string>(str, txt));
            HttpContext.Session.SetString("Dialog", JsonConvert.SerializeObject(listDialog));
        }

        public void AddToAction(string str, string txt)
        {
            var listAction = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(HttpContext.Session.GetString("Action"));
            listAction.Add(new KeyValuePair<string, string>(str, txt));
            HttpContext.Session.SetString("Action", JsonConvert.SerializeObject(listAction));
        }

        public void AddToChoices(string choice)
        {
            var sessionChoices = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("Choices"));
            List<string> listChoices = new List<string>();
            if (sessionChoices == null)
            {
                listChoices.Add(choice);
                HttpContext.Session.SetString("Choices", JsonConvert.SerializeObject(listChoices));
            }
            else
            {
                sessionChoices.Add(choice);
                HttpContext.Session.SetString("Choices", JsonConvert.SerializeObject(sessionChoices));
            }
            
        }
        public void ClearChoices()
        {
            List<string> choices = null;
            HttpContext.Session.SetString("Choices", JsonConvert.SerializeObject(choices));
        }
    }
}