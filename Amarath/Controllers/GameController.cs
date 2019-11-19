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
using Amarath.Models;

namespace Amarath.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<IdentityUserExt> userManager;

        // For coloring messages
        private string txtNormal = "#FFFFFF";
        private string txtDanger = "#FF0000";
        private string txtPlayer = "#00FFFF";
        private string txtOptions = "#66ffcc";
        private string txtSuccess = "#00FF00";
        private string txtInfo = "#FFFF00";

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
            int randNum = rand.Next(0, 2); //50% chance to spawn a monster TODO: Change later

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
            AddToAction("Lvl: " + cEnemy.Rank + "| HP: " + cEnemy.Health + "| Min Dmg: " + cEnemy.MinDamage + "| Max Dmg: " + cEnemy.MaxDamage, txtInfo);
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
            Random rand = new Random();
            int randNum = 0;
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
                        randNum = rand.Next(1, 4);
                        switch(randNum)
                        {
                            case 1:
                                AddToDialog("You decide to investigate the area...", txtNormal);
                                break;
                            case 2:
                                AddToDialog("You search the room..", txtNormal);
                                break;
                            case 3:
                                AddToDialog("You look around your surroundings...", txtNormal);
                                break;
                            default:
                                break;
                        }
                        task = GetItems;
                        break;
                    case "run":
                        AddToAction("You flee the fight!", txtNormal);
                        task = DescendLevel;
                        break;
                    case "attack":
                        task = StartBattle;
                        break;
                    case "loot":
                        AddToDialog("You loot the corpse", txtNormal);
                        task = GetItems;
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

        public async Task GetItems()
        {
            Random rand = new Random();
            int randNumofloot = rand.Next(0, 3);

            for (int i = 0; randNumofloot > i; i++)
            {
                int randloot = rand.Next(0, 23);
               // var item = db.Items.First(x => x.ItemID == randloot);

               // var loots = (from a in db.Items
                           //  where a.ItemID == randloot
                            // select a.Name);


                var cUser = await userManager.GetUserAsync(User);
                var cChar = db.Characters.First(x => x.UserId == cUser.Id);
                var item = db.Items.First(x => x.ItemID == randloot);
                var newInv = new Inventory()
                {
                    CharID = cChar.CharId,
                    ItemID = item.ItemID,
                    Quantity = 1,
                    Equiped = false
                };

                db.Inventories.Add(newInv);
                db.SaveChanges();
            }


            /*
             * Use LinQ to get items
             */
        }
        //TODO: Enemy get's regenerated, but change this later when there is time.
        public async Task StartBattle()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            int dlevel = Int32.Parse(HttpContext.Session.GetString("DungeonLevel"));
            var cEnemy = db.Enemies.First(x => x.Rank == dlevel);
            Random rand = new Random();

            Enemy enemy = cEnemy;
            while (enemy.Health > 0 && cChar.CurrentHealth > 0)
            {
                AddToAction(enemy.Name + "'s HP: " + enemy.Health, txtInfo);
                if (cChar.CurrentHealth > 0)
                {
                    var chance = rand.Next(1, 100);
                    AddToAction("You go in for an attack...", txtNormal);
                    if (chance < 10)
                    {
                        enemy.Health -= 25;
                        AddToAction("Critical! You did 25 hp of damage!", txtInfo);
                    }
                    else if (chance < 70)
                    {
                        enemy.Health -= 10;
                        AddToAction(" You did 10 hp of damage!", txtInfo);
                    }
                    else
                    {
                        AddToAction(enemy.Name + " dodged the attack!", txtInfo);
                    }
                }

                if(enemy.Health > 0)
                {
                    var chance = rand.Next(1, 100);
                    AddToAction(enemy.Name + " goes in for an attack...", txtNormal);
                    if (chance < 10)
                    {
                        cChar.CurrentHealth -= 10;
                        AddToAction("Critical! " + enemy.Name + " did 10 hp of damage!", txtInfo);
                    }
                    else if (chance < 70)
                    {
                        cChar.CurrentHealth -= 5;
                        AddToAction(enemy.Name + " did 5 hp of damage!", txtInfo);
                    }
                    else
                    {
                        AddToAction("You dodged the attack!", txtInfo);
                    }
                }
            }

            if(enemy.Health <= 0)
            {
                AddToAction("You successfully killed " + enemy.Name, txtSuccess);
                ClearChoices();
                AddToChoices("loot");
                AddToChoices("proceed");
            }
            else if(cChar.CurrentHealth <= 0)
            {
                AddToAction("You were killed by " + enemy.Name, txtDanger);
            }
        }
        public async Task EquipItem(int itemId)
        {
            AddToAction("You equiped an item!", txtNormal);
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