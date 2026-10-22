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

        public override string ToString() => $"Person ID: {PersonID}, " +
                   $"Person Name: {PersonName ?? "N/A"}, " +
                   $"Email: {Email ?? "N/A"}, " +
                   $"Gender: {Gender ?? "N/A"}, " +
                   $"Date of Birth: {(DateOfBirth.HasValue ? DateOfBirth.Value.ToString("yyyy-MM-dd") : "N/A")}, " +
                   $"Address: {Address ?? "N/A"}, " +
                   $"Country ID: {(CountryID.HasValue ? CountryID.ToString() : "N/A")}, " +
                   $"Receive News Letters: {ReceiveNewsLetters}, ";
    }
}
