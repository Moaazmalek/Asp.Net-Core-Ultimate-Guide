using Microsoft.EntityFrameworkCore;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.Services;
using Entities;
using EntityFrameworkCoreMock;
using Moq;
namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest( ) 
        {
            var countriesInitialData = new List<Country>();
            DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);


            ApplicationDbContext dbContext = dbContextMock.Object;

            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            _countriesService = new CountriesService(dbContext);
        }
        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
          
            //Assert
           await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
              //Act
             await _countriesService.AddCountry(request);
            });
        }

        //When CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest request = new()
            {
                CountryName = null
            };
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }

        //When CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsDuplicate()
        {
            //Arrange
            CountryAddRequest request1 = new()
            {
                CountryName = "USA"
            };
            CountryAddRequest request2 = new()
            {
                CountryName = "USA"
            };
           await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
               await _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
            });
        }

        //When you supply proper CountryName, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest request = new()
            {
                CountryName = "Japan"
            };
            //Act
            CountryResponse response =await _countriesService.AddCountry(request);
            //Assert
           List<CountryResponse>  countries_from_GetAllCountries=await _countriesService.GetAllCountries();
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion
        #region GetAllCountries
        // The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_from_list=await _countriesService.GetAllCountries();
            //Assert
            Assert.Empty(actual_country_from_list);

        }
        // The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> country_request_list =
            [
                new()
                {
                    CountryName="USA"
                },
                new()
                {
                    CountryName="UK"
                }
            ];

            //Acts
            List<CountryResponse> countries_list_from_add_country = [];
            foreach(CountryAddRequest country_request in country_request_list)
            {
               countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList=await _countriesService.GetAllCountries();
            //Read each element from countries_list_from_add_country
            foreach(CountryResponse expected_country in countries_list_from_add_country)
            {
                //Assert
                Assert.Contains(expected_country, actualCountryResponseList);
            }
            
        }
        #endregion
        #region GetCountryById
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            CountryResponse? country_response_from_get_method=await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);

        }
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "China"
            };

           CountryResponse  country_response_from_add_request=await _countriesService.AddCountry(country_add_request);

            //Act 
            CountryResponse? country_response_from_get=await _countriesService.GetCountryByCountryID(country_response_from_add_request.CountryID);
            //Assert 
            Assert.Equal(country_response_from_add_request, country_response_from_get);

        }
        #endregion
    }
}
