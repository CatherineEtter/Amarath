using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("Location", Schema = "dbo")]
    public class Location
    {
        [Key]
        public int LocationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set;}
        public int DungeonLevel { get; set; }
    }
}