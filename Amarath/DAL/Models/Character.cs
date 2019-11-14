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
        public string UserId { get; set; }
        public int ClassTypeId { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Experience { get; set; }
        public int DungeonLevel { get; set; }
    }
}