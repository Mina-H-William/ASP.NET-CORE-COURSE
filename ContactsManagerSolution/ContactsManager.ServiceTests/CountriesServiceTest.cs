using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        private readonly Mock<ICountriesRepository> _countryRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countryRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countryRepositoryMock.Object;

            #region DbContext Mocking (When DBContext is strongly typed to service)
            //var countriesInitialData = new List<Country>();
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //    new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(tmp => tmp.Countries, countriesInitialData);
            #endregion

            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry Tests
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arange
            CountryAddRequest? countryAddRequest = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arange
            CountryAddRequest? countryAddRequest = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public async Task AddCountry_DublicateCountryName()
        {
            //Arange
            CountryAddRequest? countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest? countryAddRequest2 = _fixture.Build<CountryAddRequest>()
                                                            .With(tmp => tmp.CountryName, countryAddRequest1.CountryName)
                                                            .Create();

            Country country1 = countryAddRequest1.ToCountry();
            Country country2 = countryAddRequest2.ToCountry();

            _countryRepositoryMock.Setup(tmp => tmp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
            _countryRepositoryMock.Setup(tmp => tmp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country1);

            await _countriesService.AddCountry(countryAddRequest1);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 // Act
                 _countryRepositoryMock.Setup(tmp => tmp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country1);
                 await _countriesService.AddCountry(countryAddRequest2);
             });
        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arange
            CountryAddRequest? countryAddRequest = _fixture.Create<CountryAddRequest>();

            Country country = countryAddRequest.ToCountry();
            CountryResponse countryResponse = country.ToCountryResponse();

            _countryRepositoryMock.Setup(tmp => tmp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            //Act
            CountryResponse response = await _countriesService.AddCountry(countryAddRequest);

            countryResponse.CountryID = response.CountryID;

            //List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            Assert.True(response.CountryID != Guid.Empty);

            //Assert.Contains(response, countries);

            Assert.Equal(countryResponse, response);
        }

        #endregion

        #region GetAllCountries Tests

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            _countryRepositoryMock.Setup(tmp => tmp.GetAllCountries()).ReturnsAsync(new List<Country>());

            // Act
            List<CountryResponse> response = await _countriesService.GetAllCountries();

            // Assert
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<Country> countries = _fixture.CreateMany<Country>(5).ToList();

            List<CountryResponse> expectedCountries = countries.Select(c => c.ToCountryResponse()).ToList();

            _countryRepositoryMock.Setup(tmp => tmp.GetAllCountries()).ReturnsAsync(countries);

            // Act
            List<CountryResponse> response = await _countriesService.GetAllCountries();

            // Assert
            response.Should().BeEquivalentTo(expectedCountries);
        }

        #endregion

        #region GetCountryByCountryID Tests

        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            // Arrange
            Guid? countryID = null;

            // Act
            CountryResponse? response = await _countriesService.GetCountryByCountryID(countryID);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidCOuntryID()
        {
            // Arrange
            Country country = _fixture.Create<Country>();
            CountryResponse countryResponse = country.ToCountryResponse();

            _countryRepositoryMock.Setup(tmp => tmp.GetCountryByCountryId(It.IsAny<Guid>())).ReturnsAsync(country);

            // Act
            CountryResponse? response = await _countriesService.GetCountryByCountryID(country.CountryID);

            // Assert
            response.Should().Be(countryResponse);
        }

        #endregion

    }
}
