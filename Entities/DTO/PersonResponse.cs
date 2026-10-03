using Entities.ServiceContracts.Enums;
using System.Runtime.CompilerServices;

namespace Entities.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return tpe 
    /// of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Address { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;

            return this.PersonID == person.PersonID && this.PersonName == person.PersonName
                && this.Email == person.Email && this.Gender == person.Gender && this.DateOfBirth == person.DateOfBirth
                && this.CountryID == person.CountryID && this.Address == person.Address
                && this.ReceiveNewsLetters == person.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
               PersonID,
               PersonName,
               Email,
               Gender,
               DateOfBirth,
               CountryID,
               Address,
               ReceiveNewsLetters
           );
        }
        public override string ToString()
        {
            return $"PersonID: {PersonID}," +
                $" PersonName: {PersonName}," +
                $" Email: {Email}, Gender: {Gender}," +
                $" DateOfBirth: {DateOfBirth}, Age: {Age}, " +
                $"Address: {Address}, CountryID: {CountryID}, " +
                $"Country: {Country}, ReceiveNewsLetters: {ReceiveNewsLetters}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest() {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth, 
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters };
        }
    }
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                Gender = person.Gender,
                DateOfBirth = person.DateOfBirth,
                Address = person.Address,
                CountryID = person.CountryID,
                Age=(person.DateOfBirth !=null) ?Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null ,
                ReceiveNewsLetters = person.ReceiveNewsLetters
            };
        }
         
    }
}
