using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        /*
         * TODO:
         * Create a Character.cs model in DAL.Models (This MUST match what is in the SQL tables exactly. The same types, the keys, etc.
         * Create a CharacterService.cs in DAL.Models (You can basically copy everything from CustomerService)
         * Reference this DBSet in AmarathContext.cs
         * Create the CreateCharacterViewModel which should store all the form controls
         * Create a method called CreateCharacter that takes in an instance of the CreateCharacterViewModel as a parameter (This is how forms are submitted, refer to the account setup)
         * - When a user submits a form by pressing the submit button, the request will go to the CreateCharacter.cshtml's CreateCharacterViewModel as an instance of the ViewModel.
         * - In this method is where the code for creating a character should go since it will have direct access to all the form data
         */
    }
}