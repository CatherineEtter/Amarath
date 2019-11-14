using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Amarath.DAL.Models
{
    public class PlayViewModel
    {
        [DataType(DataType.Text)]
        public string UserInput { get; set; }
    }
}
