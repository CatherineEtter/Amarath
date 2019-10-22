using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Amarath.DAL.Models;

namespace Amarath.Controllers
{
    public class CharacterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateCharacter()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCharacter(CharacterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //var character = new IdentityUserExt { Name = viewModel.Name, Rank = viewModel.Rank, Strength = viewModel.Strength, Intelligence = viewModel.Intelligence, Dexterity = viewModel.Dexterity, Health = viewModel.Health };
                //var result = await characterManager.CreateAsync(user, viewModel.Password);
                /*
                if (result.Succeeded)
                {
                    //TODO: Decide to use session or permanent cookie
                    //await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }*/
            }

            return View(viewModel); //for errors
        }
        /*
         * TODO: Instructions on how to make models/submission forms, and stuff
         * DONE - Create a Character.cs model in DAL.Models (This MUST match what is in the SQL tables exactly. The same types, the keys, etc.
         * DONE - Create a CharacterService.cs in DAL.Models (You can basically copy everything from CustomerService)
         * DONE - Reference the Character model DBSet in AmarathContext.cs
         * DONE - Create the CreateCharacterViewModel which should store all the form controls
         *      - Create a method called CreateCharacter that takes in an instance of the CreateCharacterViewModel as a parameter (This is how forms are submitted, pls refer to the account setup)
         *        - When a user submits a form by pressing the submit button, the request will go to the CreateCharacter.cshtml's CreateCharacterViewModel as an instance of the ViewModel.
         *        - This method is where the code for creating a character should go since it will have direct access to all the form data
         */
    }
}