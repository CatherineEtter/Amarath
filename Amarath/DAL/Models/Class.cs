using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("Class", Schema = "dbo")]
    public class Class
    {
        [Key]
        public int ClassTypeID { get; set; }
        public string Name { get; set; }
        public int Weapon { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Health { get; set; }
        public string Description { get; set; }
    }
}