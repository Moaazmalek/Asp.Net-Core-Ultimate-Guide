

using Entities.ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;  
namespace Entities.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage ="Email can't be blank")]
        [EmailAddress(ErrorMessage ="Email value should be a valid email")]
        public string? Email { get; set; }

        public GenderOptions? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public Guid? CountryID { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        /// <summary>
        /// Converts the current object of PersonAddRequest into a new object of Person object
        /// </summary>
        /// <returns></returns>
        public Person ToPerson( )
        {
            return new Person()
            {
                PersonName = PersonName,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Email = Email,
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
