using ServiceContracts.DTO;
using System.ComponentModel;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for countries.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add a country object to the list of countries.
        /// </summary>
        /// <param name="CountryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it</returns>
        CountryResponse AddCountry(CountryAddRequest? CountryAddRequest);

        /// <summary>
        /// Retrieves a list of all countries.
        /// </summary>
        /// <remarks>This method provides a complete list of countries.</remarks>
        /// <returns>A list of <see cref="CountryResponse"/> objects, where each object represents a country.</returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Returns a country based on the given country ID.
        /// </summary>
        /// <param name="countryID">CountryID (guid) to search</param>
        /// <returns>Returns a Country Response Contains country based on country id</returns>
        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}
