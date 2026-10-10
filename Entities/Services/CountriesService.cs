using Entities.DTO;
using Entities.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace Entities.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db; 
        public CountriesService(ApplicationDbContext personsDbContext)
        {
            _db = personsDbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if(await _db.Countries.AnyAsync(c => c.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            // Convert object from CountryAddRequet to Country type  
            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country.ToCountryResponse(); 
            
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async  Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;
            Country? country_response_from_list =await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == countryID);
            return country_response_from_list ==null ? null :  country_response_from_list.ToCountryResponse(); 
                }
    }
}
