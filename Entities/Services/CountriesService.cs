using Entities.DTO;
using Entities.ServiceContracts;

namespace Entities.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;
        public CountriesService(bool initialize=true)
        {
            _countries = [];
            if (initialize) {
                _countries = [
                new Country()
                {
                    CountryID = Guid.Parse("EB31273B-9F37-4774-A8BA-A806590C1FD1"),
                    CountryName = "USA"
                },
                new Country()
                {
                    CountryID = Guid.Parse("25786E47-D189-4C55-B2FB-38972A603748"),
                    CountryName = "Canada"
                },
                new Country()
                {
                    CountryID = Guid.Parse("BC7A5299-15F1-4570-AE99-6AD54D932C16"),
                    CountryName = "UK"
                },
                new Country()
                {
                    CountryID = Guid.Parse("AD14E286-CFB6-46B5-B021-735201FA708D"),
                    CountryName = "India"
                },
                new Country()
                {
                    CountryID = Guid.Parse("AD3D8A51-F4D9-44A7-BDD1-FABD229D076A"),
                    CountryName = "Australia"
                }
                ];
                
            }
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
