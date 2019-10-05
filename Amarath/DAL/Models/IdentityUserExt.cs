using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//An Extension of the IdentityUser class in ASP.NET Core Identity to add several more attributes
//for ALL Instances of IdentityUser, replace with ApplicationUser
namespace Amarath.DAL.Models
{
    public class IdentityUserExt : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
