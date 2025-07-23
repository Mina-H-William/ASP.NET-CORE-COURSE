using Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for adding a new person.
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)] // to make type in input field in view "Email"
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please Select gender of the person")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please Select a country")]
        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReciveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                Address = Address,
                CountryID = CountryID,
                DateOfBirth = DateOfBirth,
                Email = Email,
                PersonName = PersonName,
                ReciveNewsLetters = ReciveNewsLetters,
                Gender = Gender?.ToString()
            };
        }
    }
}
