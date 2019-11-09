using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("Enemy", Schema = "dbo")]
    public class Enemy
    {
        [Key]
       public int EnemyID { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int Health { get; set; }
    }
}