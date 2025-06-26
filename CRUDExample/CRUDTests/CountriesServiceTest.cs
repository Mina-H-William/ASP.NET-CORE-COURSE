using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(false);
        }
        #region AddCountry Tests
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arange
            CountryAddRequest? countryAddRequest = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = null };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public void AddCountry_DublicateCountryName()
        {
            //Arange
            CountryAddRequest? countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };


            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _countriesService.AddCountry(countryAddRequest1);
                _countriesService.AddCountry(countryAddRequest2);
            });
        }

        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "Japan" };

            //Act
            CountryResponse response = _countriesService.AddCountry(countryAddRequest);

            List<CountryResponse> countries = _countriesService.GetAllCountries();

            // Assert
            Assert.True(response.CountryID != Guid.Empty);

            Assert.Contains(response, countries);
        }

        #endregion

        #region GetAllCountries Tests

        [Fact]
        public void GetAllCountries_EmptyList()
        {
            // Act
            List<CountryResponse> response = _countriesService.GetAllCountries();

            // Assert
            Assert.Empty(response);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> countryAddRequest = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName = "USA" },
                new CountryAddRequest() { CountryName = "Canada" },
                new CountryAddRequest() { CountryName = "Mexico" }
            };

            List<CountryResponse> expectedCountries = new List<CountryResponse>();

            //foreach (var country in countryAddRequest)
            //{
            //    expectedCountries.Add(_countriesService.AddCountry(country));
            //}
            countryAddRequest.ForEach(country =>
            {
                expectedCountries.Add(_countriesService.AddCountry(country));
            });

            // Act
            List<CountryResponse> response = _countriesService.GetAllCountries();

            // Assert
            foreach (var expectedCountry in expectedCountries)
            {
                Assert.Contains(expectedCountry, response);
            }
        }

        #endregion

        #region GetCountryByCountryID Tests

        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            // Arrange
            Guid? countryID = null;

            // Act
            CountryResponse? response = _countriesService.GetCountryByCountryID(countryID);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public void GetCountryByCountryID_ValidCOuntryID()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "Germany" };
            CountryResponse country = _countriesService.AddCountry(country_add_request);
            Guid? countryID = country.CountryID;

            // Act
            CountryResponse? response = _countriesService.GetCountryByCountryID(countryID);

            // Assert
            Assert.Equal(country, response);
        }

        #endregion


    }
}
