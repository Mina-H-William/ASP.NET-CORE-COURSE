using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.UI.ModelsDTO
{
    public class RegisterDTO
    {
        [Required]
        public string PersonName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        // for Remote validation. EX: check if email is already used or not
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already used")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain numbers only")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User; // default user type is User
    }
}
