using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int InvID { get; set; }
        public int CharID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public Boolean Equiped { get; set; }
    }
}