using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;


namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly IFixture _fixture;

        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            var diagonsticContextMock = new Mock<IDiagnosticContext>();
            var loggerAdderMock = new Mock<ILogger<PersonsAdderService>>();
            var loggerGetterMock = new Mock<ILogger<PersonsGetterService>>();
            var loggerDeleterMock = new Mock<ILogger<PersonsDeleterService>>();
            var loggerUpdaterMock = new Mock<ILogger<PersonsUpdaterService>>();
            var loggerSorterMock = new Mock<ILogger<PersonsSorterService>>();

            #region How to mock dbcontext with initial data
            //var countriesInitialData = new List<Country>();
            //var personssInitialData = new List<Person>();

            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //    new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(tmp => tmp.Countries, countriesInitialData);
            //dbContextMock.CreateDbSetMock(tmp => tmp.Persons, personssInitialData);
            #endregion

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerGetterMock.Object, diagonsticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerAdderMock.Object, diagonsticContextMock.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, loggerUpdaterMock.Object, diagonsticContextMock.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, loggerDeleterMock.Object, diagonsticContextMock.Object);
            _personsSorterService = new PersonsSorterService(_personsRepository, loggerSorterMock.Object, diagonsticContextMock.Object);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson Tests

        [Fact]
        public async Task AddPerson_NullArgument()
        {
            //Arange
            PersonAddRequest? personAddRequest = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _personsService.AddPerson(personAddRequest);
            //});

            Func<Task> action = async () =>
            {
                //Act
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameNull()
        {
            //Arange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                         .With(tmp => tmp.PersonName, null as string)
                                                         .Create();

            Person person = personAddRequest.ToPerson();

            _personRepositoryMock.Setup(tmp => tmp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    //Act
            //    await _personsService.AddPerson(personAddRequest);
            //});

            Func<Task> action = async () =>
            {
                //Act
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            // difference between build and create in AutoFixture that create fill properties with random values or default values 
            // based on the type of the property but build allows you to set the properties manually by adding .with if there
            // a validation for specific property (EXample: Email)
            //Arange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                         .With(tmp => tmp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(tmp => tmp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse response = await _personsAdderService.AddPerson(personAddRequest);

            expected.PersonID = response.PersonID; // to match the ID of the response with the expected one

            //List<PersonResponse> AllPersons = await _personsService.GetAllPersons();

            //assert
            //Assert.True(response.PersonID != Guid.Empty);
            response.PersonID.Should().NotBe(Guid.Empty);

            //Assert.Contains(response, AllPersons);
            //AllPersons.Should().Contain(response);

            response.Should().Be(expected);
        }

        #endregion

        #region GetPersonByID Tests

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? response = await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            //Assert.Null(response);
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_ProperPersonID()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                                    .With(tmp => tmp.Email, "someone@gmail.com")
                                    .With(tmp => tmp.Country, null as Country)
                                    .Create();
            PersonResponse personResponse = person.ToPersonResponse();

            _personRepositoryMock.Setup(tmp => tmp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse? response = await _personsGetterService.GetPersonByPersonID(personResponse.PersonID);

            //Assert
            //Assert.Equal(personResponse, response);
            response.Should().Be(personResponse);
        }

        #endregion

        #region GetAllPersons Tests

        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            _personRepositoryMock.Setup(tmp => tmp.GetAllPersons()).ReturnsAsync(new List<Person>());

            List<PersonResponse> response = await _personsGetterService.GetAllPersons();

            //Assert.Empty(response);
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            List<Person> persons = _fixture.Build<Person>()
                                           .With(tmp => tmp.Email, "someone@gmail.com")
                                           .With(tmp => tmp.Country, null as Country)
                                           .CreateMany(5).ToList();

            List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            _personRepositoryMock.Setup(tmp => tmp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> response = await _personsGetterService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            //persons.ForEach(person => Assert.Contains(person, response));
            response.Should().BeEquivalentTo(personResponses);
        }

        #endregion

        #region GetFilteredPersons Tests

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            List<Person> persons = _fixture.Build<Person>()
                                           .With(tmp => tmp.Email, "someone@gmail.com")
                                           .With(tmp => tmp.Country, null as Country)
                                           .CreateMany(5).ToList();

            List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            _personRepositoryMock.Setup(tmp => tmp.GetAllPersons()).ReturnsAsync(persons);
            _personRepositoryMock.Setup(tmp => tmp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                                 .ReturnsAsync(persons);

            List<PersonResponse> response = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            //persons.ForEach(person => Assert.Contains(person, response));
            response.Should().BeEquivalentTo(personResponses);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            List<Person> persons = _fixture.Build<Person>()
                               .With(tmp => tmp.Email, "someone@gmail.com")
                               .With(tmp => tmp.Country, null as Country)
                               .CreateMany(5).ToList();

            List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            _personRepositoryMock.Setup(tmp => tmp.GetAllPersons()).ReturnsAsync(persons);
            _personRepositoryMock.Setup(tmp => tmp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                                 .ReturnsAsync(persons);

            List<PersonResponse> response = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            response.Should().BeEquivalentTo(personResponses);
        }

        #endregion

        #region GetSortedPersons Tests

        [Fact]
        public async Task GetSortedPersons_SortByPersonNameInDesc()
        {
            List<Person> persons = _fixture.Build<Person>()
                               .With(tmp => tmp.Email, "someone@gmail.com")
                               .With(tmp => tmp.Country, null as Country)
                               .CreateMany(5).ToList();

            List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected: ");

            persons.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            _personRepositoryMock.Setup(tmp => tmp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();

            List<PersonResponse> response = _personsSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual: ");
            response.ForEach(temp => _testOutputHelper.WriteLine(temp.ToString()));

            //persons = persons.OrderByDescending(person => person.PersonName).ToList();

            //for (int i = 0; i < persons.Count; i++)
            //{
            //    Assert.Equal(persons[i], response[i]);
            //}
            response.Should().BeInDescendingOrder(tmp => tmp.PersonName);
        }

        #endregion

        #region UpdatePerson Tests

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            PersonUpdateRequest? personUpdate = null;

            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    await _personsService.UpdatePerson(personUpdate);
            //});

            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            PersonUpdateRequest? personUpdate = _fixture.Build<PersonUpdateRequest>()
                                                        .With(tmp => tmp.Email, "email@email.com")
                                                        .Create();

            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personsService.UpdatePerson(personUpdate);
            //});

            _personRepositoryMock.Setup(tmp => tmp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);

            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdate);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            Person person = _fixture.Build<Person>()
                                    .With(tmp => tmp.Email, "someone@gmail.com")
                                    .With(tmp => tmp.Country, null as Country)
                                    .With(tmp => tmp.PersonName, null as string)
                                    .With(tmp => tmp.Gender, "Male")
                                    .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();

            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personsService.UpdatePerson(personUpdate);
            //});

            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdate);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_ProperUpdateDetails()
        {
            Person person = _fixture.Build<Person>()
                                    .With(tmp => tmp.Email, "yaho@gmail.com")
                                    .With(tmp => tmp.Country, null as Country)
                                    .With(tmp => tmp.Gender, "Male")
                                    .Create();
            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(tmp => tmp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            _personRepositoryMock.Setup(tmp => tmp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse response = await _personsUpdaterService.UpdatePerson(personUpdate);

            //Assert.Equal(updatedPerson, response);
            response.Should().Be(personResponse);
        }

        #endregion

        #region DeletePerson Tests

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            _personRepositoryMock.Setup(tmp => tmp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(false);

            var response = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert.False(response);
            response.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            Person person = _fixture.Build<Person>()
                                    .With(tmp => tmp.Email, "yaho@gmail.com")
                                    .With(tmp => tmp.Country, null as Country)
                                    .With(tmp => tmp.Gender, "Male")
                                    .Create();

            _personRepositoryMock.Setup(tmp => tmp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);

            var response = await _personsDeleterService.DeletePerson(person.PersonID);

            //Assert.True(response);
            response.Should().BeTrue();
        }

        #endregion
    }
}
