using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using modelValidationExample.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace modelValidationExample.Models
{
    public class Person : IValidatableObject
    {
        [Required(ErrorMessage = "{0} cant be null or empty")]
        [Display(Name = "Person Name")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "{0} should be between {2} and {1} characters long")]
        [RegularExpression("^[A-Za-z .]*$")]
        public string? PersonName { get; set; }

        [Range(0, 999.99, ErrorMessage = "{0} should be between ${1} and ${2}")]
        public double? Price { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        [Required]
        //[ValidateNever]  // to cancel all the validations
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }

        //[MinimumYearValidator(2005, ErrorMessage = "Date of birth should not be newer than {0}")]
        [MinimumYearValidator(2005)]
        public DateTime? DateOfBirth { get; set; }

        public DateTime? FromDate { get; set; }

        [DateRangeValidator("FromDate", ErrorMessage = "'To Date' should be newer than or equal to 'From Date'")]
        public DateTime? ToDate { get; set; }

        //[BindNever]
        public int? Age { get; set; }

        public override string ToString()
        {
            return $"person object: person name: {PersonName}, phone: {Phone},  email: {Email}, price: {Price}";
        }

        // excute only if there is no errors with the model above
        // as it return type is IEnumerable<ValidationResult> so it can return more than one ValidationResult using "yield return"
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth.HasValue == false && Age.HasValue == false)
            {
                yield return new ValidationResult("Either of Date of birth or Age should be supplied", new[] { nameof(Age) });
            }
        }
    }
}
