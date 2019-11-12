using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Amarath.Models
{
    [Table("WeaponType", Schema = "dbo")]
    public class WeaponType
    {
        [Key]
        public int WeaponTypeID { get; set; }
        public string Name { get; set; }
    }
}