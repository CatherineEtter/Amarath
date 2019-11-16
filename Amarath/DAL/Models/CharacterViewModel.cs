using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.DAL.Models
{
    public class CharacterViewModel
    {
        [Required(ErrorMessage = "Character name is required")]
        [StringLength(50, MinimumLength=1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "ClassTypeId")]
        public int ClassTypeId { get; set; }

        [Display(Name = "Rank")]
        public int Rank { get; set; }

        [Display(Name = "Strength")]
        [Range(0,100)]
        public int Strength { get; set; }

        [Display(Name = "Dexterity")]
        [Range(0, 100)]
        public int Dexterity { get; set; }

        [Display(Name = "Intelligence")]
        [Range(0, 100)]
        public int Intelligence { get; set; }

        [Display(Name = "CurrentHealth")]
        public int CurrentHealth { get; set; }

        [Display(Name = "MaxHealth")]
        public int MaxHealth { get; set; }

        [Display(Name = "Experience")]
        public int Experience { get; set; }

        [Display(Name = "DungeonLevel")]
        public int DungeonLevel { get; set; }
    }
}
