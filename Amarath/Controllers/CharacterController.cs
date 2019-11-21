using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Amarath.DAL.Models;
using Amarath.DAL.Data;
using Amarath.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Amarath.Controllers
{
    public class CharacterController : Controller
    {
        private readonly UserManager<IdentityUserExt> userManager;

        public CharacterController(UserManager<IdentityUserExt> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateCharacter()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCharacterAsync(CharacterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
                var db = new AmarathContext(optionsBuilder.Options);
                var cUser = await userManager.GetUserAsync(User);

                var character = new Character
                {
                    UserId = cUser.Id,
                    ClassTypeId = viewModel.ClassTypeId,
                    Name = viewModel.Name,
                    Rank = 1,
                    Strength = viewModel.Strength,
                    Intelligence = viewModel.Intelligence,
                    Dexterity = viewModel.Dexterity,
                    CurrentHealth = 100,
                    MaxHealth = 100,
                    Experience = 0,
                    DungeonLevel = 0,
                    TotalAttack = 0,
                    TotalDefense = 0,
                    TotalDexterity = viewModel.Dexterity,
                    TotalStrength = viewModel.Strength,
                    TotalIntelligence = viewModel.Intelligence
                };
                db.Characters.Add(character);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }
        public async Task<IActionResult> DeleteCharacter()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AmarathContext>();
            var db = new AmarathContext(optionsBuilder.Options);
            var cUser = await userManager.GetUserAsync(User);
            var cChar = db.Characters.First(x => x.UserId == cUser.Id);
            var inventories = from i in db.Inventories select i;

            foreach (Inventory inv in inventories)
            {
                if (inv.CharID == cChar.CharId)
                {
                    db.Inventories.Remove(inv);
                }
            }
            //Absolutly brutal
            db.Characters.Remove(cChar);
            db.SaveChanges();

            return RedirectToAction("Death", "Game");
        }
    }
}