using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("Character", Schema = "dbo")]
    public class Character
    {
        [Key]
        public int CharId { get; set; }
        public int UserId { get; set; }
        public int SkillListId { get; set; }
        public int InventoryId { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Inventory { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Health { get; set; }
    }
}