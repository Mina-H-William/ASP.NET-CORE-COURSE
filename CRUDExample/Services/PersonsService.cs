using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Text;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService(bool initialze = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService(initialze);
            if (initialze)
            {
                _persons.AddRange(new List<Person>()
                {
                    new Person
                    {
                        PersonID = Guid.Parse("6A96A7F3-D304-4B8A-BB84-D2EF3EA1424C"),
                        PersonName = "John Doe",
                        Email = "sample@sample.com",
                        Address = "Daher,sepo",
                        DateOfBirth = DateTime.Parse("2001-01-01"),
                        Gender = "Male",
                        ReciveNewsLetters = false,
                        CountryID = Guid.Parse("EBF9A556-6EE2-4040-B400-2AF870769A12")
                    },
                    new Person
                    {
                        PersonID = Guid.Parse("147A6C2F-44E9-40F0-A74B-32D475324802"),
                        PersonName = "Mina Hany",
                        Email = "sample@sample.com",
                        Address = "20th Settlement, Giza, Egypt",
                        DateOfBirth = DateTime.Parse("2002-02-02"),
                        Gender = "Male",
                        ReciveNewsLetters = true,
                        CountryID = Guid.Parse("977B93EB-E134-4871-AE30-7DCE755857BB")
                    },
                    new Person
                    {
                        PersonID = Guid.Parse("C0CEC26D-10F1-4C1C-B371-CF543D27B22B"),
                        PersonName = "Eslam Wageh",
                        Email = "sample@sample.com",
                        Address = "5th Settlement, New Cairo, Egypt",
                        DateOfBirth = DateTime.Parse("2003-03-03"),
                        Gender = "Male",
                        ReciveNewsLetters = true,
                        CountryID = Guid.Parse("B4A76B67-1288-4388-8B0B-939A1D7051FC")
                    },
                    new Person
                    {
                        PersonID = Guid.Parse("4D3A0BDC-F0E4-4D7B-B920-B987AF67BC79"),
                        PersonName = "moamen",
                        Email = "sample@sample.com",
                        Address = "5th Settlement, NewYork",
                        DateOfBirth = DateTime.Parse("2005-05-05"),
                        Gender = "Male",
                        ReciveNewsLetters = true,
                        CountryID = Guid.Parse("539028FD-2120-4DFA-A287-9B6A225392AB")
                    },
                    new Person
                    {
                        PersonID = Guid.Parse("58E37B40-75D6-4EF3-924D-F450E4D4173A"),
                        PersonName = "Kilian",
                        Email = "sample@sample.com",
                        Address = "5th Settlement, USA",
                        DateOfBirth = DateTime.Parse("2004-04-04"),
                        Gender = "Female",
                        ReciveNewsLetters = true,
                        CountryID = Guid.Parse("9A8FEAEF-CABE-42A4-AE8F-CA0A837CB3EC")
                    },
                    new Person
                    {
                        PersonID = Guid.Parse("080C22EB-5BE9-4C53-BAEB-7DECC4D2DC3C"),
                        PersonName = "Colo",
                        Email = "sample@sample.com",
                        Address = "5th Settlement, Inidia",
                        DateOfBirth = DateTime.Parse("2007-07-07"),
                        Gender = "Male",
                        ReciveNewsLetters = true,
                        CountryID = Guid.Parse("B4A76B67-1288-4388-8B0B-939A1D7051FC")
                    }
                });
            }
        }

        private PersonResponse ConvertPersonToPersonResponseWithCountry(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;

            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null)
                throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            _persons.Add(person);

            PersonResponse personResponse = ConvertPersonToPersonResponseWithCountry(person);

            return personResponse;
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => ConvertPersonToPersonResponseWithCountry(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID is null) return null;

            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);
            if (person is null) return null;

            return ConvertPersonToPersonResponseWithCountry(person);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchedPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchedPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.PersonName)) ?
                        person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Email)) ?
                        person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchedPersons = allPersons.Where(person => (person.DateOfBirth != null) ?
                        person.DateOfBirth.Value.ToString("dd mmmm yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)
                        .ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Gender)) ?
                        person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Country):
                    matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Country)) ?
                        person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Address)) ?
                        person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
            }

            #region filter with reflection
            //var property = typeof(PersonResponse).GetProperty(searchBy);

            //if (property == null)
            //    return allPersons;

            ////here i can check type of property with switch case and do search logic accordingly.
            ////.............

            //matchedPersons = allPersons.Where(person => property.GetValue(person)?.ToString()?
            //                 .Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            #endregion

            return matchedPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.ReciveNewsLetters).ToList(),
                (nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.ReciveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? person_Update)
        {
            if (person_Update is null)
                throw new ArgumentNullException(nameof(Person));

            ValidationHelper.ModelValidation(person_Update);

            Person? matchingPerson = _persons.FirstOrDefault(person => person_Update.PersonID == person.PersonID);

            if (matchingPerson is null)
                throw new ArgumentException($"Person with ID {person_Update.PersonID} does not exist.");

            matchingPerson.PersonName = person_Update.PersonName;
            matchingPerson.Email = person_Update.Email;
            matchingPerson.DateOfBirth = person_Update.DateOfBirth;
            matchingPerson.Address = person_Update.Address;
            matchingPerson.Gender = person_Update.Gender.ToString();
            matchingPerson.CountryID = person_Update.CountryID;
            matchingPerson.ReciveNewsLetters = person_Update.ReciveNewsLetters;

            return ConvertPersonToPersonResponseWithCountry(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID is null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);
            if (person is null)
                return false;

            _persons.RemoveAll(person => person.PersonID == personID);

            return true;
        }
    }
}