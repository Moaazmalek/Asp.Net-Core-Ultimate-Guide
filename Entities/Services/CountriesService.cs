using Entities.DTO;
using Entities.ServiceContracts;

namespace Entities.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;
        public CountriesService()
        {
            _countries = [];
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if(_countries.Where(c => c.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            // Convert object from CountryAddRequet to Country type  
            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            _countries.Add(country);
            return country.ToCountryResponse(); 
            
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;
            Country? country_response_from_list = _countries.FirstOrDefault(country => country.CountryID == countryID);
            return country_response_from_list ==null ? null :  country_response_from_list.ToCountryResponse(); 
                }
    }
}
