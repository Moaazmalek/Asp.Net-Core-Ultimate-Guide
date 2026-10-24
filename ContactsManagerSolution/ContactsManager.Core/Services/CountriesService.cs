
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;

namespace ContactsManager.Core.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository; 
        public CountriesService(ICountriesRepository countries)
        {
            _countriesRepository = countries;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if(await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            // Convert object from CountryAddRequet to Country type  
            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
             await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse(); 
            
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return [.. (await _countriesRepository.GetAll()).Select(c => c.ToCountryResponse())];
        }

        public async  Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;
            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);
            return country_response_from_list?.ToCountryResponse(); 
                }
    }
}
