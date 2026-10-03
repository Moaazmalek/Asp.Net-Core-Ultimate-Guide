
using Entities.DTO;

namespace Entities.ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object acter adding it (including newly generated country id) </returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as List<CountryResponse> </returns>
        List<CountryResponse> GetAllCountries();
        /// <summary>
        /// Return Country object based on the given country id from the list
        /// </summary>
        /// <param name="countryID" </param>
        /// <returns>Matching country as CountryResponse object</returns>
        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}
