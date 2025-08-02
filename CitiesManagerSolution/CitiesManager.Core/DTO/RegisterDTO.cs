using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string PersonName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(dataType: DataType.EmailAddress)]
        ///
        /// Remote attribute used to make a ajax call to check function in controller so it works only with views 
        /// in apis its not working, so we can use custom validation attribute instead
        ///
        //[Remote(action: "IsEmailAlreadyRegistered", controller: "Account")] // not working with Apicontroller 
        public string Email { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Should Match With Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain digits only")]
        [DataType(dataType: DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

    }
}
