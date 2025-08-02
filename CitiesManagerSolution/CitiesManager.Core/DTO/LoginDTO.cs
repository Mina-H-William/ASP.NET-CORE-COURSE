using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
