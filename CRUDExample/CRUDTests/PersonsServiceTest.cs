using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonsService(false);
            _countriesService = new CountriesService(false);
            _testOutputHelper = testOutputHelper;
        }

        private List<PersonResponse> AddPersonsToTest()
        {
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                CountryName = "Egypt"
            };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Address = "sample address",
                CountryID = country1.CountryID,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Email = "mo@mo.com",
                Gender = GenderOptions.Female,
                PersonName = "Mary",
                ReciveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Address = "sample address",
                CountryID = country2.CountryID,
                DateOfBirth = DateTime.Parse("2002-01-09"),
                Email = "sample@sample.com",
                Gender = GenderOptions.Male,
                PersonName = "Rahman",
                ReciveNewsLetters = true
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                Address = "sample address",
                CountryID = country2.CountryID,
                DateOfBirth = DateTime.Parse("2002-09-09"),
                Email = "sample@sample.com",
                Gender = GenderOptions.Male,
                PersonName = "moamen",
                ReciveNewsLetters = true
            };

            List<PersonResponse> persons = new List<PersonResponse>
            {
                _personsService.AddPerson(personAddRequest1),
                _personsService.AddPerson(personAddRequest2),
                _personsService.AddPerson(personAddRequest3)
            };

            return persons;
        }

        #region AddPerson Tests

        [Fact]
        public void AddPerson_NullArgument()
        {
            //Arange
            PersonAddRequest? personAddRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_PersonNameNull()
        {
            //Arange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "mina",
                Email = "mina309hany@gmail.com",
                Address = "sample address",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2002-01-01"),
                ReciveNewsLetters = true
            };

            //Act
            PersonResponse response = _personsService.AddPerson(personAddRequest);

            List<PersonResponse> AllPersons = _personsService.GetAllPersons();

            //assert
            Assert.True(response.PersonID != Guid.Empty);
            Assert.Contains(response, AllPersons);
        }

        #endregion

        #region GetPersonByID Tests

        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? response = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(response);
        }

        [Fact]
        public void GetPersonByPersonID_ProperPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mina",
                Address = "sample Address",
                CountryID = country.CountryID,
                DateOfBirth = DateTime.Parse("2002-01-01"),
                Email = "sample@sample.com",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true
            };
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            Guid personID = personResponse.PersonID;

            //Act
            PersonResponse? response = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Equal(personResponse, response);
        }

        #endregion

        #region GetAllPersons Tests

        [Fact]
        public void GetAllPersons_EmptyList()
        {
            List<PersonResponse> response = _personsService.GetAllPersons();

            Assert.Empty(response);
        }

        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            List<PersonResponse> persons = AddPersonsToTest();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> response = _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            persons.ForEach(person => Assert.Contains(person, response));
        }

        #endregion

        #region GetFilteredPersons Tests

        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            List<PersonResponse> persons = AddPersonsToTest();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> response = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            persons.ForEach(person => Assert.Contains(person, response));
        }

        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            List<PersonResponse> persons = AddPersonsToTest();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> response = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            persons.ForEach(person =>
            {
                if (person.PersonName?.Contains("ma", StringComparison.OrdinalIgnoreCase) ?? false)
                    Assert.Contains(person, response);
            });
        }

        #endregion

        #region GetSortedPersons Tests

        [Fact]
        public void GetSortedPersons_SortByPersonNameInDesc()
        {
            List<PersonResponse> persons = AddPersonsToTest();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            List<PersonResponse> response = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            persons = persons.OrderByDescending(person => person.PersonName).ToList();

            for (int i = 0; i < persons.Count; i++)
            {
                Assert.Equal(persons[i], response[i]);
            }
        }

        #endregion

        #region UpdatePerson Tests

        [Fact]
        public void UpdatePerson_NullPerson()
        {
            PersonUpdateRequest? personUpdate = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.UpdatePerson(personUpdate);
            });
        }

        [Fact]
        public void UpdatePerson_NullPersonID()
        {
            PersonUpdateRequest? personUpdate = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };


            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.UpdatePerson(personUpdate);
            });
        }

        [Fact]
        public void UpdatePerson_NullPersonName()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mina",
                Address = "sample Address",
                CountryID = country.CountryID,
                DateOfBirth = DateTime.Parse("2002-01-01"),
                Email = "sample@gmail.com",
                Gender = GenderOptions.Male,
            };
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();
            personUpdate.PersonName = null;

            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.UpdatePerson(personUpdate);
            });
        }

        [Fact]
        public void UpdatePerson_ProperUpdateDetails()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mina",
                Address = "sample Address",
                CountryID = country.CountryID,
                DateOfBirth = DateTime.Parse("2002-01-01"),
                Email = "sample@gmail.com",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true
            };
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();
            personUpdate.PersonName = "Updated Mina";
            personUpdate.Email = "mina@gmail.com";

            PersonResponse response = _personsService.UpdatePerson(personUpdate);

            PersonResponse? updatedPerson = _personsService.GetPersonByPersonID(response.PersonID);

            Assert.Equal(updatedPerson, response);
        }

        #endregion

        #region DeletePerson Tests

        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            Assert.False(_personsService.DeletePerson(Guid.NewGuid()));
        }

        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mina",
                Address = "sample Address",
                CountryID = country.CountryID,
                DateOfBirth = DateTime.Parse("2010-01-01"),
                Email = "sample@gmail.com",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true
            };
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            Assert.True(_personsService.DeletePerson(personResponse.PersonID));
        }

        #endregion

    }
}
