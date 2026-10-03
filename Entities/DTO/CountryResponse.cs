 
namespace Entities.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        // Override this method so Solve the problem of Assert.Contains(..)
        // It compares the current object to another object of CountResponse type and returns
        // true, if both values are same; otherwise returns false;
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(CountryResponse)) return false;

            CountryResponse responseObject = (CountryResponse)obj;
            return this.CountryName == responseObject.CountryName &&
                   this.CountryID == responseObject.CountryID;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(CountryID, CountryName);
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName
            };
        }
       
    }
}
