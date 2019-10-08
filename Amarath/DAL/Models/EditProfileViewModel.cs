using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Amarath.DAL.Models
{
    public class EditProfileViewModel
    {  
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password must not be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
