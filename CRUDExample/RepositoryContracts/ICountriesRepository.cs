using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access operations for countries.
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to the data store
        /// </summary>
        /// <param name="country">country object to add</param>
        /// <returns></returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Retrieves a list of all available countries.
        /// </summary>
        /// <returns>All Countries from table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Retrieves a country by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the country to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the  <see cref="Country"/>
        /// object if found; otherwise, <see langword="null"/>.</returns>
        Task<Country?> GetCountryByCountryId(Guid id);

        /// <summary>
        /// Retrieves a country by its name.
        /// </summary>
        /// <param name="countryName">The name of the country to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Country"/> object
        /// if a match is found; otherwise, <see langword="null"/>.</returns>
        Task<Country?> GetCountryByCountryName(string countryName);


    }
}
