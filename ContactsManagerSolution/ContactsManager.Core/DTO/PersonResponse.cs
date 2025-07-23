using Entities;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }

        public string? PersonName { get; set; }

        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Country { get; set; }

        public double? Age { get; set; }

        public string? Address { get; set; }

        public bool ReciveNewsLetters { get; set; }

        public string? TIN { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse other = (PersonResponse)obj;

            return PersonID == other.PersonID &&
                   PersonName == other.PersonName &&
                   Email == other.Email &&
                   DateOfBirth == other.DateOfBirth &&
                   Gender == other.Gender && CountryID == other.CountryID &&
                     Address == other.Address && ReciveNewsLetters == other.ReciveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Gender: {Gender}, Email: {Email}" +
                $", Date of Birth: {DateOfBirth?.ToString("dd mm yyyy")}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                ReciveNewsLetters = ReciveNewsLetters,
                Address = Address,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, true),
                CountryID = CountryID,
            };
        }
    }

    public static class PersonResponseExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReciveNewsLetters = person.ReciveNewsLetters,
                Age = (person.DateOfBirth != null) ?
                      Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                      : null,
                TIN = person.TIN,
                Country = person.Country?.CountryName ?? string.Empty
            };
        }
    }
}
