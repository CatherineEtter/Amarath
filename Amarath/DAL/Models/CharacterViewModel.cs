using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.DAL.Models
{
    public class CharacterViewModel
    {
        [Display(Name = "Rank")]
        public int Rank { get; set; }
        [Display(Name = "Strength")]
        public int Strength { get; set; }
        [Display(Name = "Dexterity")]
        public int Dexterity { get; set; }
        [Display(Name = "Dexterity")]
        public int Intelligence { get; set; }
        [Display(Name = "Health")]
        public int CurrentHealth { get; set; }

        [Required(ErrorMessage = "Character name is required")]
        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
