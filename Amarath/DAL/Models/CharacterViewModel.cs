using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.DAL.Models
{
    public class CharacterViewModel
    {
        [Display(Name = "ClassTypeId")]
        public int ClassTypeId { get; set; }
        [Display(Name = "Rank")]
        public int Rank { get; set; }
        [Display(Name = "Strength")]
        public int Strength { get; set; }
        [Display(Name = "Dexterity")]
        public int Dexterity { get; set; }
        [Display(Name = "Dexterity")]
        public int Intelligence { get; set; }
        [Display(Name = "CurrentHealth")]
        public int CurrentHealth { get; set; }
        [Display(Name = "MaxHealth")]
        public int MaxHealth { get; set; }
        [Display(Name = "Experience")]
        public int Experience { get; set; }
        [Display(Name = "DungeonLevel")]
        public int DungeonLevel { get; set; }

        [Required(ErrorMessage = "Character name is required")]
        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
