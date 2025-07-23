using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();

            _countriesService = _countriesServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> personsResponse = _fixture.Create<List<PersonResponse>>();

            _personsGetterServiceMock.Setup(tmp => tmp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync(personsResponse);

            _personsSorterServiceMock.Setup(tmp => tmp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(),
                                                                  It.IsAny<SortOrderOptions>())).Returns(personsResponse);

            PersonsController personsController = new PersonsController(_countriesService, _logger,
                                                                        _personsAdderService, _personsDeleterService,
                                                                        _personsUpdaterService, _personsSorterService,
                                                                        _personsGetterService);

            //Act
            IActionResult response = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(response);

            // BeAssignableTo checks if the model is of type IEnumerable<PersonResponse> or its children
            // viewResult.ViewData.Model is the model that passed to the view in strongly typed view
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(personsResponse);
        }

        #endregion

        #region Create

        // this unit test case giving error due to shifting code for modelvalidation from controller to filter
        // so this test case is commented out as the responsibility of model validation is now on filter
        #region Not Working
        //[Fact]
        //public async Task Create_IfModelErrors_ToReturnCreateView()
        //{
        //    //Arrange
        //    PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        //    PersonResponse personResponse = _fixture.Create<PersonResponse>();
        //    List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();

        //    _countriesServiceMock.Setup(tmp => tmp.GetAllCountries()).ReturnsAsync(countryResponses);

        //    _personsServiceMock.Setup(tmp => tmp.AddPerson(It.IsAny<PersonAddRequest>()))
        //                       .ReturnsAsync(personResponse);

        //    PersonsController personsController = new PersonsController(_countriesService, _personsService, _logger);

        //    //Act
        //    personsController.ModelState.AddModelError("Person Name", "Person Name Can't be blank");
        //    IActionResult response = await personsController.Create(personAddRequest);

        //    //Assert
        //    ViewResult viewResult = Assert.IsType<ViewResult>(response);

        //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        //    viewResult.ViewData.Model.Should().BeEquivalentTo(personAddRequest);
        //}
        #endregion

        [Fact]
        public async Task Create_IfNoModelErrors_RedirectToIndexAction()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(tmp => tmp.GetAllCountries()).ReturnsAsync(countryResponses);

            _personsAdderServiceMock.Setup(tmp => tmp.AddPerson(It.IsAny<PersonAddRequest>()))
                               .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_countriesService, _logger,
                                                                        _personsAdderService, _personsDeleterService,
                                                                        _personsUpdaterService, _personsSorterService,
                                                                        _personsGetterService);

            //Act
            IActionResult response = await personsController.Create(personAddRequest);

            //Assert
            RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(response);

            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Persons");
        }

        #endregion
    }
}
