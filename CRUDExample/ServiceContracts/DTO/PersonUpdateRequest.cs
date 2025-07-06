using Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required]
        public Guid PersonID { get; set; }

        [Required]
        public string? PersonName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

        [Required]
        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReciveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                PersonID = PersonID,
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
