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
    }
}
