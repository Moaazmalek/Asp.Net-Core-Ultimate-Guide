using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new Country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);
        /// <summary>
        /// Returns all Countries in the data store
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAll();
        /// <summary>
        /// Returns country object based on the given country id;
        /// otherwise, it return null
        /// </summary>
        /// <param name="countryID">CountrID to search </param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);

        /// <summary>
        /// Returns country object based on the given Country Name;
        /// otherwise, it return null
        /// </summary>
        /// <param name="countryName">CountryName to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
