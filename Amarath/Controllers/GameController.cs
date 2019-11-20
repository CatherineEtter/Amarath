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
        public IActionResult Death()
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

            if(cChar.CurrentHealth <= 0)
            {
                await DeleteCharacter();
                return View("Death");
            }
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
                await UpdateTotals();
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
            int randNum = rand.Next(0, 100); //50% chance to spawn a monster TODO: Change later

            ClearChoices();
            //Don't spawn anything in level 0
            if (Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel"))  == 0)
            {
                AddToChoices("proceed");
                AddToDialog(" - Proceed", txtOptions);
            }
            else if(randNum < 20)
            {
                AddToChoices("explore");
                AddToChoices("proceed");
                
                AddToDialog("You enter and look around.", txtNormal);
                AddToDialog(" - Explore", txtOptions);
                AddToDialog(" - Proceed", txtOptions);
                
                if(Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel")) != 1)
                {
                    AddToChoices("back");
                    AddToDialog(" - Back", txtOptions);
                }
            }
            else
            {
                SpawnEnemies();
            }
        }

        public ActionResult SpawnEnemies()
        {
            int dlevel = Int32.Parse(HttpContext.Session.GetString("DungeonLevel"));
            var cEnemy = db.Enemies.First(x => x.Rank == dlevel);

            AddToChoices("attack");
            AddToAction("A " + cEnemy.Name + " appears!", txtDanger);
            AddToAction("Lvl: " + cEnemy.Rank + "| HP: " + cEnemy.Health + "| Min Dmg: " + cEnemy.MinDamage + "| Max Dmg: " + cEnemy.MaxDamage, txtInfo);
            AddToDialog(" - Attack", txtOptions);
            if(Convert.ToInt32(HttpContext.Session.GetString("DungeonLevel")) != 1)
            {
                AddToChoices("run");
                AddToDialog(" - Run", txtOptions);
            }

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
                        AddToDialog("You move on...", txtNormal);
                        task = AscendLevel;
                        break;
                    case "back":
                        AddToDialog("You decide to go back", txtNormal);
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
                        AddToDialog("You decide to attack...", txtNormal);
                        task = StartBattle;
                        break;
                    case "loot":
                        AddToDialog("You loot the corpse", txtNormal);
                        task = GetItems;
                        break;
                    case "accept fate":
                        AddToDialog("You have accepted your fate...", txtDanger);
                        Task.Run(async () => { await DeleteCharacter(); }).Wait();
                        return View("Death");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                AddToDialog(viewModel.UserInput + " is not a valid choice!", txtDanger);
            }
            //Call the action down here because the Session Dialog needs to be set so it doesn't get overwritten
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

        public async Task<IActionResult> GetItems()
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

                AddToAction("You found a(n) " + item.Name + "!", txtSuccess);

                db.Inventories.Add(newInv);
            }
            if(randNumofloot == 0)
            {
                AddToAction("You didn't find anything...",txtNormal);
            }
            db.SaveChanges();

            ClearChoices();
            AddToChoices("back");
            AddToChoices("proceed");
            AddToDialog(" - Back", txtOptions);
            AddToDialog(" - Proceed", txtOptions);

            return View("Play");
        }
        //TODO: Enemy get's regenerated, but change this later when there is time.
        public async Task StartBattle()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            int dlevel = Int32.Parse(HttpContext.Session.GetString("DungeonLevel"));
            var cEnemy = db.Enemies.First(x => x.Rank == dlevel);
            Random rand = new Random();
            Enemy enemy = new Enemy();
            enemy.Name = cEnemy.Name;
            enemy.Health = cEnemy.Health;
            enemy.MinDamage = cEnemy.MinDamage;
            enemy.MaxDamage = cEnemy.MaxDamage;

            //Strength modifies TotalAttack
            //Intelligence modifies loot chance and critical attack
            //Dexterity affects dodge chance
            while (enemy.Health > 0 && cChar.CurrentHealth > 0)
            {
                AddToAction(enemy.Name + "'s HP: " + enemy.Health, txtInfo);
                //Player attacks
                if (cChar.CurrentHealth > 0)
                {
                    //Max crit chance is 60%
                    //Base enemy dodge is 10%
                    var critChance = 10 + cChar.Intelligence > 60 ? 60 : 10 + cChar.Intelligence;
                    var hitChance = rand.Next(1, 100);
                    var hitValue = cChar.TotalAttack;

                    AddToAction("You go in for an attack...", txtNormal);
                    if (hitChance < critChance)
                    {
                        var crit = hitValue + rand.Next(5, 20);
                        enemy.Health -= crit;
                        AddToAction("Critical! You did " + crit + " hp of damage!", txtInfo);
                    }
                    else if (hitChance < critChance + 10)
                    {
                        AddToAction(enemy.Name + " dodged the attack!", txtInfo);
                    }
                    else
                    {
                        enemy.Health -= hitValue;
                        AddToAction(" You did " + hitValue + " hp of damage!", txtInfo);
                    }
                }
                //Enemy attacks
                if(enemy.Health > 0)
                {
                    //Max dodge chance is 70
                    //10% chance for enemy crit
                    var dodgeChance = 10 + cChar.Dexterity > 70 ? 70 : 10 + cChar.Dexterity;
                    var hitChance = rand.Next(1, 100);
                    var hitValue = rand.Next(enemy.MinDamage, enemy.MaxDamage + 1);

                    AddToAction(enemy.Name + " goes in for an attack...", txtNormal);
                    if (hitChance < dodgeChance)
                    {
                        AddToAction("You dodged the attack!", txtInfo);
                    }
                    else if (hitChance < dodgeChance + 10)
                    {
                        var crit = enemy.MaxDamage + rand.Next(1, 10);
                        cChar.CurrentHealth -= crit;
                        AddToAction("Critical! " + enemy.Name + " did " + crit + " hp of damage!", txtDanger);
                    }
                    else
                    {
                        cChar.CurrentHealth -= 5;
                        AddToAction(enemy.Name + " did " + hitValue + " hp of damage!", txtDanger);
                    }
                }
            }
            db.SaveChanges();

            if(enemy.Health <= 0)
            {
                AddToAction("You successfully killed " + enemy.Name, txtSuccess);
                ClearChoices();
                AddToChoices("loot");
                AddToChoices("proceed");
                AddToDialog(" - Loot", txtOptions);
                AddToDialog(" - Proceed", txtOptions);

            }
            if(cChar.CurrentHealth <= 0)
            {
                AddToDialog("You have died!", txtDanger);
                AddToAction("You have died!", txtDanger);
                AddToAction("You were killed by " + enemy.Name, txtDanger);
                ClearChoices();
                AddToChoices("accept fate");
                AddToDialog("- Accept Fate", txtOptions);
            }
        }
        public async Task<IActionResult> EquipItem(int inventoryId)
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var invId = db.Inventories.FirstOrDefault(x => x.InvID == inventoryId);
            var selectedItem = db.Items.FirstOrDefault(x => x.ItemID == invId.ItemID);

            var equippedItems = from x in db.Inventories where (x.Equiped == true && x.CharID == cChar.CharId) select x; //Get list of all equiped items

            if(!invId.Equiped)
            {
                foreach (Inventory invItem in equippedItems)
                {
                    var currentItem = db.Items.FirstOrDefault(x => x.ItemID == invItem.ItemID);
                    if (selectedItem.Type == currentItem.Type) //If player tries to equip two unique items
                    {
                        AddToAction("You are already wearing a " + selectedItem.Type, txtInfo);
                        return View("Play");
                    }
                }
                invId.Equiped = true;
                AddToAction("You equiped the " + selectedItem.Name, txtNormal);
            }
            else
            {
                invId.Equiped = false;
                AddToAction("You unequipped the " + selectedItem.Name, txtNormal);
            }
            db.SaveChanges();
            await UpdateTotals();

            return View("Play");
        }

        public async Task<IActionResult> UpdateTotals()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var equippedItems = from x in db.Inventories where (x.Equiped == true && x.CharID == cChar.CharId) select x;

            //Reset Character's totals
            cChar.TotalStrength = cChar.Strength;
            cChar.TotalDexterity = cChar.Dexterity;
            cChar.TotalIntelligence = cChar.Intelligence;
            cChar.TotalDefense = 0;
            cChar.TotalAttack = 0;

            foreach (Inventory inv in equippedItems)
            {
                var currentItem = db.Items.FirstOrDefault(x => x.ItemID == inv.ItemID);
                cChar.TotalStrength += currentItem.Strength;
                cChar.TotalDexterity += currentItem.Dexterity;
                cChar.TotalIntelligence += currentItem.Intelligence;
                cChar.TotalDefense += currentItem.Defense;
                cChar.TotalAttack += currentItem.Damage;
            }
            cChar.TotalAttack += Convert.ToInt32(Math.Floor(cChar.TotalStrength * .20));
            db.SaveChanges();
            return View("Play");
        }
        public async Task<IActionResult> UseItem(int inventoryId)
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var invId = db.Inventories.FirstOrDefault(x => x.InvID == inventoryId);
            var item = db.Items.FirstOrDefault(x => x.ItemID == invId.ItemID);
            AddToAction("You used " + item.Name, txtNormal);

            if(item.Name.Equals("Health Potion"))
            {
                var hp = 0;
                if(cChar.MaxHealth - cChar.CurrentHealth < 20)
                {
                    hp = cChar.MaxHealth - cChar.CurrentHealth;
                }
                else
                {
                    hp = 20;
                }
                AddToAction(hp + " hp restored", txtSuccess);
                cChar.CurrentHealth += hp;
            }
            if(invId.Quantity <= 1)
            {
                db.Inventories.Remove(invId);
            }
            else
            {
                invId.Quantity -= 1;
            }
            db.SaveChanges();
            return View("Play");
        }
        public async Task<ViewResult> DeleteCharacter()
        {
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var inventories = from i in db.Inventories select i;

            foreach(Inventory inv in inventories)
            {
                if(inv.CharID == cChar.CharId)
                {
                    db.Inventories.Remove(inv);
                }
            }
            //Absolutly brutal
            db.Characters.Remove(cChar);
            db.SaveChanges();
            return View("Death");
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