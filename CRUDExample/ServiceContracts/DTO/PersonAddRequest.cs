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
        [Required]
        public string? PersonName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

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
