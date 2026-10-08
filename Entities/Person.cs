
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }
        [StringLength(40)] // nvarchar(40)
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        // unique identifier
        public Guid? CountryID { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public string? TIN { get; set; } //text identification Number
        [ForeignKey("CountryID")]
        public Country? Country { get; set; }


    }
}
